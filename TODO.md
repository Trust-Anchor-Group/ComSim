TODO
========

Features
-------------

[x] Events
	[x] Stochastic events
	[x] External events
	[x] Trigger activities
	[x] Guards
	[x] Clocks
[x] Meta-data
	[x] Title
	[x] Description
	[x] Script
	[x] Section descriptions
[ ] Activities
	[x] Populations
	[x] Execution
	[ ] Activity node with external event conditions, or stochastic conditions
	[x] Counters
	[x] Stat activity node: Increments counters
	[ ]	Sensor data (local/remote)
	[x] Measurements
	[x] Performance counters
	[x] CPU/Memory
	[x] Global variables
	[ ] Timing of, and between events/activities. (communication roundtrip)
[ ] Simulation output
	[ ] Statistics
		[x] Activities
		[x] Actions
		[x] Events
		[x] Counters
		[x] Histograms
		[x] Mean, stddev, var, min, max
		[x] Errors, Exceptions (from Log)
		[x] Event type histogram over time
		[ ] Custom SQL
	[x] Charts
		[x] History charts
		[x] Use case charts
		[x] Distribution (expected) charts
	[x] Data as XML
	[x] Report as Markdown
		[x] Distributions
		[x] Models
		[x] Activity charts
		[x] Statistics
		[x] Custom graphs
		[x] Event log statistics
[ ] Distributions
	[ ] Script-based PDF and CDF
	[ ] Arcsine distribution
	[ ] Beta distribution
	[ ] Chi distribution
	[ ] Chi-squared distribution
	[ ] Exponential distribution
	[ ] Gamma distribution
	[ ] Student-t distribution
[x] Generate action documentation (xml, xslt) automatically
[ ] XMPP
	[ ] Message handlers
	[ ] Iq handlers
	[ ] Presence handlers
	[ ] Extension libraries
	[ ] UDP
	[ ] Iq action node
	[ ] Message action node
	[ ] Presence action node
[ ] HTTP
	[x] GET
	[x] POST
		[x] String
		[x] JSON
		[ ] Xml
		[ ] Signed XML
	[ ] HEAD
	[ ] PUT
	[ ] DELETE
	[ ] Web Service (SOAP)
[ ] MQTT
	[ ] PUBLISH
	[ ] SUBSCRIBE

Examples
--------------

[x] Simple chat
[x] Guess a number
[x] Web Server Load (HTTP GET)
[ ] Federation
[ ] External events to determine activity node transitions
[ ] Parallel threads in activity
[ ] Timing of communication roundtrip

Documentation
--------------------

[ ] Structure of a simulation file
[ ] TimeBase, TimeUnit, TimeCycle, Duration (cyclic time)
[ ] Explain keys

