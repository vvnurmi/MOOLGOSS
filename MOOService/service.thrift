# Thrift service definition for MOOLGOSS.

namespace csharp MOO.Service

typedef i32 ID

struct DateTime {
	1: i32 Year
	2: byte Month
	3: byte Day
	4: byte Hour
	5: byte Minute
}

struct TimeSpan {
    1: i32 Milliseconds
}

struct Location {
	1: ID Planet
}

struct UpdateData {
    1: DateTime Stardate
    2: TimeSpan UpdateInterval
    3: TimeSpan NextUpdate
}

struct Planet {
    1: ID ID
    2: optional string Player
    3: string Name
    4: i32 Population
    5: i32 MaxPopulation
    6: i32 Orbit
}

struct Formation {
    1: ID ID
    2: string Player
    3: Location Location
    4: i32 Ships
}

enum CommandType {
	MoveFormation = 1
}

struct MoveFormationData {
	1: ID Formation
	2: Location Destination
}

struct Command {
	1: CommandType Type
	2: optional MoveFormationData MoveFormationData
}

service MOOService {
    void Authenticate(1: string name)
    UpdateData GetUpdate()
    list<Planet> GetPlanets()
    list<Formation> GetFormations()
    void IssueCommand(1: Command command)
}
