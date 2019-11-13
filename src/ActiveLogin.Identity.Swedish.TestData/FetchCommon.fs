module FetchCommon


let private parseAsTuple (s:string) =
    (s.[ 0..3 ], s.[ 4..5 ], s.[ 6..7 ], s.[ 8..10 ], s.[ 11.. 11 ])

let private tupleAsString (year, month, day, birthNumber, checksum) =
    sprintf "            (%s,%s,%s,%s,%s)" year month day birthNumber checksum

let toStringTuple = parseAsTuple >> tupleAsString


let appendLine (sb:System.Text.StringBuilder) line = sb.AppendLine line |> ignore


