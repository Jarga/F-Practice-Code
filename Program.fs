open agsXMPP
open System;
open ChatterBotAPI

type CleverbotXmppClient(jid: Jid, password) =
    do  printfn "Started Creating CleverbotXmppClient %s: " (jid.ToString())

    let mutable connection: XmppClientConnection = new XmppClientConnection(jid, password)
    let mutable cleverBotSession: ChatterBotSession = (new ChatterBotFactory()).Create(ChatterBotType.CLEVERBOT).CreateSession()

    let Reply(replyToMessage: protocol.client.Message, connection: XmppClientConnection, cleverBotSession: ChatterBotSession) = 
        if String.IsNullOrWhiteSpace(replyToMessage.Body) then
            None
        else
            let reply = cleverBotSession.Think(replyToMessage.Body)
            let message = new protocol.client.Message(replyToMessage.From, protocol.client.MessageType.chat, reply)

            Some(connection.Send(message))
          
    

    let OnMessage(e: protocol.client.Message) = 
        printfn "Message from %s: %s " (e.From.ToString()) e.Body
        let result = Reply(e, connection, cleverBotSession)
        if result.IsSome then
            result.Value
        else
            result |> ignore
        

    let OnLogin(e) = printfn "Logged in successfully"
    let OnLoginFailed(e: Xml.Dom.Element) = printfn "Login failed"

    do connection.AutoResolveConnectServer = true |> ignore
    do connection.OnMessage.Add(OnMessage)
    do connection.OnLogin.Add(OnLogin)
    do connection.OnAuthError.Add(OnLoginFailed)

    do connection.Open()
            
    do printfn "Finished Creating CleverbotXmppClient %s: " (jid.ToString())


[<EntryPoint>]
let main argv = 
    let username = "Jarga.Anza"
    let server = "gmail.com"
    let password = ""
    let port = 5222
    
    let jid = new Jid(username + "@" + server)

    let client = new CleverbotXmppClient(jid, password)

    while true do Async.Sleep(2000) |> ignore

    printfn "%A" argv
    0 // return an integer exit code
