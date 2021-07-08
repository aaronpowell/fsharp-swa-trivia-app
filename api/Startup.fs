module Startup

open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Newtonsoft.Json.Converters

type Startup() =
    inherit FunctionsStartup()

    override _.Configure(_: IFunctionsHostBuilder) : unit =
        let settings = JsonSerializerSettings()
        settings.ContractResolver <- CamelCasePropertyNamesContractResolver()

        DiscriminatedUnionConverter()
        |> settings.Converters.Add

        StringEnumConverter() |> settings.Converters.Add

        JsonConvert.DefaultSettings <- (fun _ -> settings)

[<assembly: FunctionsStartup(typeof<Startup>)>]
do ()
