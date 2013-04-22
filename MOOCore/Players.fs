module MOO.Core.Players

open MOO.Core.State
open MOO.Core.Types

let hasPlanets player =
    state {
        let! planets = getPlanets
        let planets = List.map snd <| Map.toList planets
        return planets |> List.exists (fun p -> p.player = Some player)
    }
let initPlayer name =
    state {
        let! planets = getPlanets
        let freePlanets = planets |> Map.filter (fun id p -> p.player = None)
        if not freePlanets.IsEmpty then
            let planet = snd <| (Map.toList freePlanets).[0]
            do! setPlanet { planet with player = Some name; population = 42 }
            do! addFormation { id = 0; player = name; location = Planet planet.id; ships = 1 }
    }
