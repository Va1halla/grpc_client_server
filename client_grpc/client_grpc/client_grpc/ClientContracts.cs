using System;
using System.Threading.Tasks;
using Grpc.Core;
using server_grpc;

namespace contracts
{
    public class ClientContracts
    {
        const int port = 5001;
        public static async Task<string> TestConnection(bool Signal)
        {
            try
            {
                Channel channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
                Handle.HandleClient client = new Handle.HandleClient(channel);
                server_Signal response = await client.signalToServerAsync(new client_Signal { Signal = Signal });
                if (response.Signal == true)
                {
                    return await Task.FromResult("Connected. Ready for hacking...");
                }
                else
                {
                    return await Task.FromResult("Unknown error...");
                }

            }
            catch (RpcException e)
            {
                return await Task.FromResult(e.ToString());
                //"Unable to connect gRPC server..."
            }
        }
        public static async Task<string> AddingPersons(string _FirstName, string _SecondName, int _Age)
        {
            PersonValues input = new PersonValues { FirstName = _FirstName, SecondName = _SecondName, Age = _Age };
            try
            {
                //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                Channel channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
                AddPerson.AddPersonClient client = new AddPerson.AddPersonClient(channel);
                dbAnswer response = await client.SendValuesAsync(input);
                if (response.Error != "None")
                {
                    return await Task.FromResult($"{response.Error}");
                }
                else
                {
                    return await Task.FromResult($"Id {response.Id}");
                }
            }
            catch (RpcException e)
            {
                return await Task.FromResult("Unable to connect gRPC server...");
            }
        }

        public static async Task<string> PersonsInfo(int _Id)
        {
            PersonRequest request = new PersonRequest { Id = _Id };
            try
            {
                //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                Channel channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
                GetPersonById.GetPersonByIdClient client = new GetPersonById.GetPersonByIdClient(channel);
                PersonResponse response = await client.GetPersonAsync(request);
                if (response.Error != "None")
                {
                    return await Task.FromResult($"{response.Error}");
                }
                else
                {
                    return await Task.FromResult($"Id {response.Id}\nFirst Name {response.FirstName}\nSecond Name {response.SecondName}\nAge {response.Age}");
                }

            }
            catch (RpcException e)
            {
                return await Task.FromResult("Unable to connect gRPC server...");
            }
        }
    }
}
