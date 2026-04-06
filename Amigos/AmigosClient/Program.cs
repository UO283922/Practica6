using System.Net.Http.Headers;

namespace AmigosClient
{
    internal class Program
    {
        // Solo un HttpClient reutilizado en todo el programa
        static HttpClient client = new HttpClient();
       
        static void ConfigureClient()
        {
            client.BaseAddress = new Uri("http://localhost:5145");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json") );
        }

        static void InsertFriends()
        {
            // Lista de amigos a insertar
            List<Amigo> newFriends = new List<Amigo>
            {
                new Amigo { name = "Rubén",   longi = "123",  lati = "456" },
                new Amigo { name = "María",   longi = "-3.7038",  lati = "40.4168" },
                new Amigo { name = "Carlos",  longi = "-0.3763",  lati = "39.4699" },
                new Amigo { name = "Lucía",   longi = "-8.7207",  lati = "42.2328" },
                new Amigo { name = "Javier",  longi = "-1.9812",  lati = "43.3183" }
            };

            Console.WriteLine("=== INSERTANDO AMIGOS ===\n");

            foreach (Amigo amigo in newFriends)
            {
                HttpResponseMessage response = client.PostAsJsonAsync("api/amigo", amigo).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Amigo '{amigo.name}' creado con éxito. URL: {response.Headers.Location}");
                }
                else
                {
                    Console.WriteLine($"Error al crear '{amigo.name}': {response.StatusCode}");
                }
            }
        }

        static void ShowFriendList()
        {
            Console.WriteLine("\n=== LISTA DE AMIGOS ===\n");
            HttpResponseMessage response = client.GetAsync("api/amigo").Result;
            if (response.IsSuccessStatusCode)
            {
                Amigo[] amigos = response.Content.ReadAsAsync<Amigo[]>().Result;
                foreach (Amigo amigo in amigos)
                    Console.WriteLine("{0}: {1}", amigo.name, amigo.ID);
            }
            else
            {
                Console.WriteLine($"Error al obtener la lista: {response.StatusCode}");
            }
        }

        static void ModifyFriend(int id, string? nuevoNombre = null, string? nuevaLongi = null, string? nuevaLati = null)
        {
            Console.WriteLine($"\n=== MODIFICANDO AMIGO ID: {id} ===\n");

            // Primero comprobamos que el amigo existe
            HttpResponseMessage responseGet = client.GetAsync($"api/amigo/{id}").Result;

            if (!responseGet.IsSuccessStatusCode)
            {
                Console.WriteLine($"✘ No se encontró ningún amigo con ID {id}.");
                return;
            }

            Amigo amigo = responseGet.Content.ReadAsAsync<Amigo>().Result;
            Console.WriteLine($"  Amigo encontrado: '{amigo.name}' | Lon: {amigo.longi} | Lat: {amigo.lati}");

            // Solo se modifica el campo si se ha proporcionado un valor
            if (nuevoNombre != null) amigo.name = nuevoNombre;
            if (nuevaLongi != null) amigo.longi = nuevaLongi;
            if (nuevaLati != null) amigo.lati = nuevaLati;

            HttpResponseMessage response = client.PutAsJsonAsync($"api/amigo/{id}", amigo).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Amigo modificado con éxito.");
            }
            else
            {
                Console.WriteLine($"Error al modificar el amigo: {response.StatusCode}");
            }
        }

        static void DeleteFriend(int id)
        {
            Console.WriteLine($"\n=== BORRANDO AMIGO DE ID: {id} ===\n");

            // Primero comprobamos que el amigo existe
            HttpResponseMessage responseGet = client.GetAsync($"api/amigo/{id}").Result;

            if (!responseGet.IsSuccessStatusCode)
            {
                Console.WriteLine($"✘ No se encontró ningún amigo con ID {id}.");
                return;
            }

            Amigo amigo = responseGet.Content.ReadAsAsync<Amigo>().Result;
            Console.WriteLine($"  Amigo encontrado: '{amigo.name}' | Lon: {amigo.longi} | Lat: {amigo.lati}");

            HttpResponseMessage responseDelete = client.DeleteAsync($"api/amigo/{id}").Result;

            if (responseDelete.IsSuccessStatusCode)
            {
                Console.WriteLine($"Amigo '{amigo.name}' borrado con éxito.");
            }
            else
            {
                Console.WriteLine($"Error al borrar el amigo: {responseDelete.StatusCode}");
            }
        }

        static void Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");
            ConfigureClient();

            try
            {
                InsertFriends();
                //ModifyFriend(35, "Javi", null, "15");
                //DeleteFriend(31);
                ShowFriendList();
                
            } catch (Exception ex)
            {
                Console.WriteLine("Excepcion durante ejecucion", ex.Message);
            }

            Console.WriteLine("\nPulsa cualquier tecla para salir...");
            Console.ReadKey();
            

        }
    }
}
