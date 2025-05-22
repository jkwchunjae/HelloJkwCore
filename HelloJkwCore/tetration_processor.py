import numpy as np
import requests
import base64
from io import BytesIO
from PIL import Image
from dataclasses import dataclass
from typing import Tuple, Optional
from numba import jit, prange
import time
import json

@dataclass
class TePoint:
    x: float
    y: float

@dataclass
class TeSize:
    width: int
    height: int

@dataclass
class TeRectangle:
    left_top: TePoint
    right_bottom: TePoint

@dataclass
class TeOptions:
    max_iterations: int
    divergence_radius: float
    eps_x: float

@dataclass
class TetrationTask:
    task_id: str
    allocated: bool
    rectangle: TeRectangle
    image_size: TeSize
    options: TeOptions

class TetrationProcessor:
    def __init__(self, base_url: str = "https://hellojkwcore.azurewebsites.net"):
        self.base_url = base_url
        self.session = requests.Session()

    def get_tetration_task(self) -> Optional[TetrationTask]:
        try:
            response = self.session.get(f"{self.base_url}/api/Hello/tetration/task")
            if response.status_code == 200:
                data = response.json()
                return TetrationTask(
                    task_id=data['taskId'],
                    allocated=data['allocated'],
                    rectangle=TeRectangle(
                        left_top=TePoint(data['rectangle']['leftTop']['x'], data['rectangle']['leftTop']['y']),
                        right_bottom=TePoint(data['rectangle']['rightBottom']['x'], data['rectangle']['rightBottom']['y'])
                    ),
                    image_size=TeSize(data['imageSize']['width'], data['imageSize']['height']),
                    options=TeOptions(
                        data['options']['maxIterations'],
                        data['options']['divergenceRadius'],
                        data['options']['epsX']
                    )
                )
        except Exception as e:
            print(f"Error getting task: {e}")
        return None

    def send_image(self, task: TetrationTask, image_base64: str):
        try:
            response = self.session.post(
                f"{self.base_url}/api/Hello/tetration/result",
                json={
                    "taskId": task.task_id,
                    "base64Image": image_base64
                }
            )
            print(f"Response: {response.status_code}")
            return response.status_code == 200
        except Exception as e:
            print(f"Error sending image: {e}")
            return False

    def send_progress(self, task: TetrationTask, image_base64: str, progress: int, total: int):
        try:
            response = self.session.post(
                f"{self.base_url}/api/Hello/tetration/progress",
                json={
                    "taskId": task.task_id,
                    "base64Image": image_base64,
                    "progress": progress,
                    "total": total
                }
            )
            return response.status_code == 200
        except Exception as e:
            print(f"Error sending progress: {e}")
            return False

@jit(nopython=True, parallel=True)
def compute_tetration_divergence(real_range: np.ndarray, imag_range: np.ndarray, 
                               max_iterations: int, escape_radius: float) -> np.ndarray:
    nx, ny = len(real_range), len(imag_range)
    divergence_map = np.zeros((nx, ny), dtype=np.bool_)
    
    for i in prange(nx):
        for j in range(ny):
            c_real = real_range[i]
            c_imag = imag_range[j]
            z_real = c_real
            z_imag = c_imag
            diverged = False
            
            for k in range(max_iterations):
                # 복소수 제곱 계산을 실수부와 허수부로 분리
                z_magnitude = np.sqrt(z_real * z_real + z_imag * z_imag)
                if z_magnitude > escape_radius:
                    diverged = True
                    break
                
                # z = c^z 계산
                # c^z = exp(z * log(c))
                # log(c) = log|c| + i*arg(c)
                c_magnitude = np.sqrt(c_real * c_real + c_imag * c_imag)
                c_arg = np.arctan2(c_imag, c_real)
                
                log_c_real = np.log(c_magnitude)
                log_c_imag = c_arg
                
                # z * log(c) 계산
                temp_real = z_real * log_c_real - z_imag * log_c_imag
                temp_imag = z_real * log_c_imag + z_imag * log_c_real
                
                # exp(z * log(c)) 계산
                exp_real = np.exp(temp_real)
                z_real = exp_real * np.cos(temp_imag)
                z_imag = exp_real * np.sin(temp_imag)
                
                # 발산 체크
                if not np.isfinite(z_real) or not np.isfinite(z_imag):
                    diverged = True
                    break
            
            divergence_map[i, j] = diverged
    
    return divergence_map

def save_bool_array_as_image(data: np.ndarray) -> str:
    # 데이터를 전치(transpose)하여 가로 방향으로 올바르게 표시
    data = data.T
    
    # Convert boolean array to image
    img = Image.fromarray(data.astype(np.uint8) * 255)
    
    # Save to bytes buffer
    buffer = BytesIO()
    img.save(buffer, format='PNG')
    
    # 이미지 파일명 생성 (현재 시간 기준)
    filename = f"tetration.png"
    
    import os
    if os.path.exists(filename):
        os.remove(filename)
    # 이미지 파일로 저장
    img.save(filename)
    print(f"이미지가 {filename}로 저장되었습니다.")
    
    # Convert to base64
    return base64.b64encode(buffer.getvalue()).decode('utf-8')

def process_tetration_task(task: TetrationTask) -> str:
    # Create coordinate ranges
    real_range = np.linspace(
        task.rectangle.left_top.x,
        task.rectangle.right_bottom.x,
        task.image_size.width
    )
    imag_range = np.linspace(
        task.rectangle.left_top.y,
        task.rectangle.right_bottom.y,
        task.image_size.height
    )
    
    # Compute divergence map
    divergence_map = compute_tetration_divergence(
        real_range,
        imag_range,
        task.options.max_iterations,
        task.options.divergence_radius
    )

    print(f"Divergence map shape: {divergence_map.shape}")
    
    # Convert to image and return base64
    base64_image = save_bool_array_as_image(divergence_map)
    return base64_image

def main():
    processor = TetrationProcessor()
    print("Starting Tetration Processor")
    
    while True:
        try:
            task = processor.get_tetration_task()
            if task and task.task_id != None:
                print(f"Task details:")
                print(f"  Task ID: {task.task_id}")
                print(f"  Rectangle: ({task.rectangle.left_top.x}, {task.rectangle.left_top.y}) to ({task.rectangle.right_bottom.x}, {task.rectangle.right_bottom.y})")
                print(f"  Image size: {task.image_size.width}x{task.image_size.height}")
                print(f"  Max iterations: {task.options.max_iterations}")
                print(f"  Divergence radius: {task.options.divergence_radius}")
                print(f"  Allocated: {task.allocated}")

                print(f"Processing task {task.task_id}")
                image_base64 = process_tetration_task(task)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_progress(task, image_base64, 0, 100)
                time.sleep(0.1)
                processor.send_image(task, image_base64)
            else:
                time.sleep(1)
        except Exception as e:
            print(f"Error in main loop: {e}")
            time.sleep(1)

if __name__ == "__main__":
    main() 