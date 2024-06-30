open System;
open System.Net;
open System.Net.Http;
open System.IO;

printfn "FoxWebServer Application";

let Code = 0
let mutable Port = 5000
let mutable server = Net.HttpListener();
let mutable quit = false;

let ProcessRequset (context: Net.HttpListenerContext ) =
        let sw = StreamWriter (context.Response.OutputStream)
        sw.WriteLine ("<html>");
        sw.WriteLine ("<head>");
        sw.WriteLine ("<meta charset=Utf-8>");
        sw.WriteLine ("</head>");
        sw.WriteLine ("<body>");
        sw.WriteLine ("Приложение на F# в форме  web-сервиса");

        sw.WriteLine ("<div class=menu>");
        for i = 1 to 5 do
            sw.WriteLine ($"<a href='/?page={i}'>{i}</a>");
        sw.WriteLine ("</div>");
        
        
        if context.Request.QueryString.Keys.Count>0  then
            printfn($"Передано кол-во параметров {context.Request.QueryString.Keys} / {context.Request.QueryString.Keys[0]}" )
            let page : int = Int32.Parse (context.Request.QueryString.GetValues("page")[0])
            sw.WriteLine ($"Выбрана страница {page}");

        sw.WriteLine ("</body>");
        sw.WriteLine ("</html>");
        sw.Flush();
        sw.Close();
        0

[<EntryPoint>]
let  main args =
        printfn "Переданные аргументы"
        for a in args do
            printfn $"{a} "
            Port <- Int32.Parse(a);//номер порта для сервера
    
        printfn $"Установленный порт {Port}"
        server.Prefixes.Add ($"http://localhost:{Port}/")
        server.Start();
        printfn $"Принимаю подключения..."
        (* Шаблон сервера*)
        let AsyncWork = 
                    async 
                        {
                            let! context  =  server.GetContextAsync()  |>Async.AwaitTask
                            printfn $"Новый запрос: {context.Request.Url}  кол-во cookie = {context.Request.Cookies.Count}"
                            ProcessRequset context 
                        }

        //Цикл обработки запроса
        while quit=false do
            try 
                let work = AsyncWork |> Async.StartAsTask
                true
            with 
                | :? System.Exception as ex -> printfn $"Сервер сломался! {ex.StackTrace}";  quit<-true; false ;
        
        //Сервер стоп
        server.Close()
        Code //Возвращаемое значение фунцкии (иначе не работает)