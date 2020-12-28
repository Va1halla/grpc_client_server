// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Core;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace server_grpc

{
    class AddPersonImpl : AddPerson.AddPersonBase
    {
        private readonly ILogger<AddPersonImpl> logger;

        public AddPersonImpl()
        {
        }

        public AddPersonImpl(ILogger<AddPersonImpl> logger)
        {
            this.logger = logger;
        }

        public override async Task<dbAnswer> SendValues(PersonValues request, ServerCallContext context)
        {
            try
            {
                string ConnectionString = "Host=172.17.0.1;Username=postgres;Password=postgres;Database=postgres";
                await using NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                await Connection.OpenAsync();
                await using NpgsqlCommand Command = new NpgsqlCommand($"INSERT INTO public.persons(first_name, last_name, age) VALUES(\'{ request.FirstName }\',\'{ request.SecondName}\',{request.Age}) RETURNING Id", Connection);
                await using NpgsqlDataReader Reader = await Command.ExecuteReaderAsync();
                await Reader.ReadAsync();
                int response = Reader.GetInt32(0);
                return await Task.FromResult(new dbAnswer { Id = response, Error = "None" });
            }
            catch (Npgsql.PostgresException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Failed to complete query, check connection string or input valid arguments.");
                return await Task.FromResult(new dbAnswer { Id = 0, Error = "Query failed" });
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("Selected Host does not exists.");
                Console.WriteLine(e.ToString());
                return await Task.FromResult(new dbAnswer { Id = 0, Error = "Host does not exists" });
            }
        }
    }

    class GetPersonImpl : GetPersonById.GetPersonByIdBase
    {
        private readonly ILogger<GetPersonImpl> logger;

        public GetPersonImpl()
        {
        }

        public GetPersonImpl(ILogger<GetPersonImpl> logger)
        {
            this.logger = logger;
        }

        public override async Task<PersonResponse> GetPerson(PersonRequest request, ServerCallContext context)
        {
            async Task<string> AnswerFromDB_String(string col)
            {
                string ConnectionString = "Host=172.17.0.1;Username=postgres;Password=postgres;Database=postgres";
                try
                {
                    await using NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                    await using NpgsqlCommand Command = new NpgsqlCommand($"SELECT {col} FROM public.persons WHERE id={request.Id}", Connection);
                    await using NpgsqlDataReader Reader = await Command.ExecuteReaderAsync();
                    await Reader.ReadAsync();
                    var response = Reader.GetString(0);
                    return response;
                }
                catch (Npgsql.PostgresException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Failed to complete query, check connection string or input valid arguments.");
                    return "Query failed";
                }
                catch (Npgsql.NpgsqlException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Connection to DB was interrupted");
                    return "Connection to DB was interrupted";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Selected Host does not exists.");
                    return "Host does not exists";
                }
                catch (System.InvalidOperationException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine($"Row {request.Id} not found");
                    return "";
                }


            }
            async Task<int> AnswerFromDB_Int(string col)
            {
                string ConnectionString = "Host=172.17.0.1;Username=postgres;Password=postgres;Database=postgres";
                try
                {
                    await using NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                    await Connection.OpenAsync();
                    await using NpgsqlCommand Command = new NpgsqlCommand($"SELECT {col} FROM public.persons WHERE id={request.Id}", Connection);
                    await using NpgsqlDataReader Reader = await Command.ExecuteReaderAsync();
                    await Reader.ReadAsync();
                    var response = Reader.GetInt32(0);
                    return response;
                }
                catch (Npgsql.PostgresException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Failed to connect database. Please check your credentials or use existing database.");
                    return -1;
                }
                catch (Npgsql.NpgsqlException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Connection to DB was interrupted");
                    return -3;
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Selected Host does not exists.");
                    return -2;
                }
                catch (System.InvalidOperationException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine($"Row {request.Id} not found");
                    return 0;
                }

            }
            if (string.IsNullOrEmpty(await AnswerFromDB_String("first_name")) == true && string.IsNullOrEmpty(await AnswerFromDB_String("last_name")) == true && string.IsNullOrEmpty(await AnswerFromDB_String("age")))
            {
                return await Task.FromResult(new PersonResponse { FirstName = "", SecondName = "", Age = 0, Id = request.Id, Error = "Not Found" });
            }
            else if ((await AnswerFromDB_String("first_name") == "Connection to DB was interrupted") || (await AnswerFromDB_String("last_name") == "Connection to DB was interrupted") || (await AnswerFromDB_Int("age") == -3))
            {
                return await Task.FromResult(new PersonResponse { FirstName = null, SecondName = null, Age = 0, Id = request.Id, Error = "Query failed" });
            }
            else if ((await AnswerFromDB_String("first_name") == "Query failed") || (await AnswerFromDB_String("last_name") == "Query failed") || (await AnswerFromDB_Int("age") == -1))
            {
                return await Task.FromResult(new PersonResponse { FirstName = null, SecondName = null, Age = 0, Id = request.Id, Error = "Query failed" });
            }
            else if ((await AnswerFromDB_String("first_name") == "Host does not exists") || (await AnswerFromDB_String("last_name") == "Host does not exists") || (await AnswerFromDB_Int("age") == -2))
            {
                return await Task.FromResult(new PersonResponse { FirstName = null, SecondName = null, Age = 0, Id = request.Id, Error = "Host does not exists" });
            }
            else
            {
                return await Task.FromResult(new PersonResponse { FirstName = await AnswerFromDB_String("first_name"), SecondName = await AnswerFromDB_String("last_name"), Age = await AnswerFromDB_Int("age"), Error = "None", Id = request.Id });
            }
        }
    }
    class HandlerImpl: Handle.HandleBase
    {
        private readonly ILogger<HandlerImpl> logger;

        public HandlerImpl()
        {
        }

        public HandlerImpl(ILogger<HandlerImpl> logger)
        {
            this.logger = logger;
        }
        public override async Task<server_Signal> signalToServer(client_Signal request, ServerCallContext context)
        {
            if(request.Signal == true)
            {
                return await Task.FromResult(new server_Signal { Signal = true });
            }
            else
            {
                Program.Shutdown.Set(); // <--- Signals the main thread to continue 
                return await Task.FromResult(new server_Signal { Signal = false });
            }
            
        }
    }

    class Program
    {
        const int Port = 5001;
        public static ManualResetEvent Shutdown = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { AddPerson.BindService(new AddPersonImpl()) , GetPersonById.BindService(new GetPersonImpl()), Handle.BindService(new HandlerImpl())},
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server listening on port " + Port);
            Shutdown.WaitOne();
            server.ShutdownAsync().Wait() ;
        }
    }
}
