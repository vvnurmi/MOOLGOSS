module MOO.Types

type ID = int

type Planet = {
    id : ID
    name : string
    population : int
    maxPopulation : int
    orbit : int
}
