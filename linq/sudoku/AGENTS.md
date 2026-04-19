# sudoku assist POC project

https://hodoku.sourceforge.net/ 를 참고해 스도쿠를 풀어주는 프로그램을 만든다.

# 객체 설명

## IBoard

SudokuModel.linq

스도쿠의 전체 보드를 의미.
9*9 칸으로 이뤄져있음.
IHouse를 갖고 있음.

## IHouse

SudokuModel.linq

9칸 구역을 의미.
1부터 9까지 한 번만 등장해야 함.

IBoard 는 총 27개의 IHouse를 가지고 있음.
가로 9개, 세로 9개, 3*3 정사각형 9개.

## ICell

SudokuModel.linq

스도쿠 한 칸을 의미.
숫자 하나를 써야 함.
숫자는 비어 있을 수 있고, 비어있다면 후보 숫자를 가지고 있음.

## IStrategy

IStrategy.linq

스도쿠 전략 interface

## IValueStrategy

IStrategy.linq

값을 찾는 전략.
이 전략을 사용하면 셀의 값을 찾을 수 있다.

하나의 StrategyResult 를 구할 수 있다.
값이 구해진다면 그 셀에 값을 찾았다는 뜻이다.

## ICandidateStrategy

IStrategy.linq

셀의 후보 중 지울 수 있는 후보를 찾는다.
StrategyResult 목록을 반환한다.
목록에 있는 후보는 삭제할 수 있다.

## 전략 요약

- **NakedSingleStrategy** (SingleStrategy.linq): 후보가 하나뿐인 빈 셀을 찾아 해당 숫자를 확정한다.
- **HiddenSingleStrategy** (SingleStrategy.linq): 특정 하우스에서 어떤 숫자가 단 하나의 셀에만 후보로 존재하면 그 셀 값을 확정한다.
- **FullHouseStrategy** (SingleStrategy.linq): 한 하우스에 빈 셀이 하나만 남았을 때 남은 숫자를 채워 넣는다.
- **NakedSubsetStrategy** (NakedSubsetStrategy.linq): 동일 후보 집합을 공유하는 셀 조합을 찾고, 그 조합 외 셀에서 해당 후보를 제거한다.
- **HiddenSubsetStrategy** (HiddenSubsetStrategy.linq): 하우스에서 특정 후보들이 정확히 동일한 셀 집합에만 존재할 때, 그 셀들의 다른 후보를 제거한다.
- **IntersectionRowColumnStrategy** (IntersectionStrategy.linq): 행 또는 열에서 특정 후보가 하나의 블록 안에만 분포할 경우, 같은 블록의 다른 하우스에서 그 후보를 제거한다.
- **IntersectionBlockStrategy** (IntersectionStrategy.linq): 블록 내부 후보들이 단일 행/열에 집중될 때, 동일 행/열의 다른 블록 셀에서 그 후보를 제거한다.
- **BasicFishStrategyBase / XWingFishStrategy / SwordfishStrategy / JellyfishStrategy** (BasicFishStrategy.linq): 2~4개의 행 또는 열에서 특정 후보가 정확히 같은 수의 반대 축 하우스에만 걸리는 Basic Fish 패턴을 찾고, 그 반대 축의 다른 셀에서 해당 후보를 제거한다.
- **SkyscraperStrategy** (SkyscraperStrategy.linq): 두 개의 행 또는 열 conjugate pair 가 하나의 공통 열/행을 밑변으로 공유할 때, 나머지 두 끝점을 함께 보는 후보를 제거한다.
- **TwoStringKiteStrategy** (TwoStringKiteStrategy.linq): 하나의 행 conjugate pair 와 하나의 열 conjugate pair 가 같은 블록 안에서 연결될 때, 두 끝점을 함께 보는 후보를 제거한다.
- **TurbotFishStrategy** (TurbotFishStrategy.linq): 블록 conjugate pair 와 행/열 conjugate pair 를 strong-weak-strong 형태의 4개 후보 X-Chain 으로 연결해 끝점 둘을 함께 보는 후보를 제거한다.
