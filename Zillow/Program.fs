open FSharp.Data
// invoke this with Zillow API ID followed by a list of zillow property ids on command line
// expect list of address/zestimates printed, then repeated with estimates alone

type Zestimate = XmlProvider<"""
    <zestimate>
      <response>
        <address>
          <street>1 High Street</street>
          <zipcode>12345</zipcode>
          <city>Town</city>
          <state>DC</state>
        </address>
        <zestimate>
          <amount currency="USD">1</amount>
        </zestimate>
      </response>
    </zestimate>
    """>

[<EntryPoint>]
let main argv =
    let zwsId = Array.head argv
    let ids = argv.[1..]
    let url = "https://www.zillow.com/webservice/GetZestimate.htm"

    let shower (z:Zestimate.Zestimate) = 
        printfn "%s:\t$%d"
            z.Response.Address.Street
            z.Response.Zestimate.Amount.Value

    let show_less (z:Zestimate.Zestimate) = printfn "$%d" z.Response.Zestimate.Amount.Value

    let get_zest (id:string) =
        Http.RequestString ( url, query = [ "zws-id", zwsId; "zpid", id ] )
        |> Zestimate.Parse

    let zests = Array.map get_zest ids
    Seq.iter shower zests
    Seq.iter show_less zests

    0   // return an integer exit code
