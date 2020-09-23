{
	"cpuPercent":PerformanceCounters.GetCounter("Processor","_Total","% Processor Time").NextValue(),
	"networkBytesPerSec":PerformanceCounters.GetCounter("Network Interface","Microsoft Hyper-V Network Adapter","Bytes Total/sec").NextValue(),
	"memoryMBytesFree":PerformanceCounters.GetCounter("Memory","Available MBytes").NextValue()
}