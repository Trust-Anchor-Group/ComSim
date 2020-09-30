{
	"cpuPercent":PerformanceCounters.GetCounter("Processor","_Total","% Processor Time").NextValue(),
	"networkBytesPerSec":PerformanceCounters.GetCounter("Network Interface","Microsoft Hyper-V Network Adapter","Bytes Total/sec").NextValue(),
	"memoryMBytesFree":PerformanceCounters.GetCounter("Memory","Available MBytes").NextValue(),
	"xmppCpu":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","% Processor Time").NextValue(),
	"mqttCpu":PerformanceCounters.GetCounter("Process","mosquitto","% Processor Time").NextValue(),
	"mqCpu":PerformanceCounters.GetCounter("Process","runmqlsr","% Processor Time").NextValue()+
		PerformanceCounters.GetCounter("Process","amqsvc","% Processor Time").NextValue(),
	"xmppMemory":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","Working Set - Private").NextValue(),
	"mqttMemory":PerformanceCounters.GetCounter("Process","mosquitto","Working Set - Private").NextValue(),
	"mqMemory":PerformanceCounters.GetCounter("Process","runmqlsr","Working Set - Private").NextValue()+
		PerformanceCounters.GetCounter("Process","amqsvc","Working Set - Private").NextValue(),
	"xmppIo":PerformanceCounters.GetCounter("Process","Waher.IoTGateway.Svc","IO Data Bytes/sec").NextValue(),
	"mqttIo":PerformanceCounters.GetCounter("Process","mosquitto","IO Data Bytes/sec").NextValue(),
	"mqIo":PerformanceCounters.GetCounter("Process","runmqlsr","IO Data Bytes/sec").NextValue()+
		PerformanceCounters.GetCounter("Process","amqsvc","IO Data Bytes/sec").NextValue()
}