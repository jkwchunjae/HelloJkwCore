﻿namespace ProjectPingpong;

public class PingpongOption
{
    public FileSystemSelectOption FileSystemSelect { get; set; } = new();
    public PathMap Path { get; set; } = new();
}
