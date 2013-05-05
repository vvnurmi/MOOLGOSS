# Thrift service definition for MOOLGOSS.

namespace csharp MOO.Service
namespace as3 com.moolgoss.service

typedef i32 ID

struct DateTime {
    1: i32 year
    2: byte month
    3: byte day
    4: byte hour
    5: byte minute
}

struct TimeSpan {
    1: i32 milliseconds
}

struct Location {
    1: ID planet
}

struct UpdateData {
    1: DateTime stardate
    2: TimeSpan updateInterval
    3: TimeSpan nextUpdate
}

struct Planet {
    1: ID id
    2: optional string player
    3: string name
    4: i32 population
    5: i32 maxPopulation
    6: i32 orbit
}

struct Formation {
    1: ID id
    2: string player
    3: Location location
    4: i32 ships
}

enum CommandType {
    MoveFormation = 1
}

struct MoveFormationData {
    1: ID formation
    2: Location destination
}

struct Command {
    1: CommandType type
    2: optional MoveFormationData moveFormationData
}

service MOOService {
    void Authenticate(1: string name)
    UpdateData GetUpdate()
    list<Planet> GetPlanets()
    list<Formation> GetFormations()
    void IssueCommand(1: Command command)
}
