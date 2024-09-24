using System.Net;
using System.Text.Json;

namespace AppleDeviceCSV
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string apiUrl = "https://api.restful-api.dev/objects";
            string csvFilePath = "devices.csv";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";
                request.Timeout = 10000;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    var devices = JsonSerializer.Deserialize<List<Device>>(result);

                    if (devices is null)
                        throw new Exception("Nenhum resultado encontrado");

                    var appleDevices = devices.Where(d => d.Brand.ToLower() == "apple");

                    using (var writer = new StreamWriter(csvFilePath))
                    {
                        writer.WriteLine("Nome,Preço");
                        foreach (var device in appleDevices)
                        {
                            writer.WriteLine($"{device.Name},{device.Price}");
                        }
                    }
                }

                Console.WriteLine("Arquivo CSV gerado com sucesso!");
            }
            catch (WebException webEx)
            {
                Console.WriteLine($"Houve um erro ao acessar a API: {webEx.Message}");
                if (webEx.Response is HttpWebResponse errorResponse)
                {
                    Console.WriteLine($"Código de status: {(int)errorResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }
        }
    }
}
