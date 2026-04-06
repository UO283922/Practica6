using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AmigosClient
{
    internal class Program
    {
        // Solo un HttpClient reutilizado en todo el programa
        static HttpClient client = new HttpClient();

        static void ConfigureClient()
        {
            client.BaseAddress = new Uri("http://localhost:5145/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        static void ShowFriendList()
        {
            Console.WriteLine("\n=== LISTA DE AMIGOS ===\n");

            HttpResponseMessage response = client.GetAsync("api/amigo").Result;
            if (response.IsSuccessStatusCode)
            {
                Amigo[] amigos = response.Content.ReadAsAsync<Amigo[]>().Result;

                if (amigos.Length == 0)
                {
                    Console.WriteLine("No hay amigos almacenados.");
                    return;
                }

                foreach (Amigo amigo in amigos)
                {
                    Console.WriteLine($"ID: {amigo.ID} | Nombre: {amigo.name} | Longitud: {amigo.longi} | Latitud: {amigo.lati}");
                }
            }
            else
            {
                Console.WriteLine($"Error al obtener la lista: {response.StatusCode}");
            }
        }

        static void ShowFriendById()
        {
            Console.WriteLine("\n=== VER AMIGO POR ID ===\n");
            Console.Write("Introduce el ID: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            HttpResponseMessage response = client.GetAsync($"api/amigo/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                Amigo amigo = response.Content.ReadAsAsync<Amigo>().Result;
                Console.WriteLine($"\nID: {amigo.ID}");
                Console.WriteLine($"Nombre: {amigo.name}");
                Console.WriteLine($"Longitud: {amigo.longi}");
                Console.WriteLine($"Latitud: {amigo.lati}");
            }
            else
            {
                Console.WriteLine("Amigo no encontrado.");
            }
        }

        static void CreateFriend()
        {
            Console.WriteLine("\n=== CREAR AMIGO ===\n");

            Amigo amigo = new Amigo();

            Console.Write("Nombre: ");
            amigo.name = Console.ReadLine();

            Console.Write("Longitud: ");
            amigo.longi = Console.ReadLine();

            Console.Write("Latitud: ");
            amigo.lati = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(amigo.name) ||
                string.IsNullOrWhiteSpace(amigo.longi) ||
                string.IsNullOrWhiteSpace(amigo.lati))
            {
                Console.WriteLine("Todos los campos son obligatorios.");
                return;
            }

            HttpResponseMessage response = client.PostAsJsonAsync("api/amigo", amigo).Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Amigo '{amigo.name}' creado con éxito. URL: {response.Headers.Location}");
            }
            else
            {
                Console.WriteLine($"Error al crear el amigo: {response.StatusCode}");
            }
        }

        static void ModifyFriendInteractive()
        {
            Console.WriteLine("\n=== MODIFICAR AMIGO ===\n");
            Console.Write("Introduce el ID del amigo a modificar: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            HttpResponseMessage responseGet = client.GetAsync($"api/amigo/{id}").Result;

            if (!responseGet.IsSuccessStatusCode)
            {
                Console.WriteLine($"No se encontró ningún amigo con ID {id}.");
                return;
            }

            Amigo amigo = responseGet.Content.ReadAsAsync<Amigo>().Result;

            Console.WriteLine($"\nActual:");
            Console.WriteLine($"Nombre: {amigo.name}");
            Console.WriteLine($"Longitud: {amigo.longi}");
            Console.WriteLine($"Latitud: {amigo.lati}");

            Console.Write("\nNuevo nombre (enter para mantener): ");
            string? nuevoNombre = Console.ReadLine();

            Console.Write("Nueva longitud (enter para mantener): ");
            string? nuevaLongi = Console.ReadLine();

            Console.Write("Nueva latitud (enter para mantener): ");
            string? nuevaLati = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nuevoNombre)) amigo.name = nuevoNombre;
            if (!string.IsNullOrWhiteSpace(nuevaLongi)) amigo.longi = nuevaLongi;
            if (!string.IsNullOrWhiteSpace(nuevaLati)) amigo.lati = nuevaLati;

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

        static void DeleteFriendInteractive()
        {
            Console.WriteLine("\n=== BORRAR AMIGO ===\n");
            Console.Write("Introduce el ID del amigo a borrar: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            HttpResponseMessage responseGet = client.GetAsync($"api/amigo/{id}").Result;

            if (!responseGet.IsSuccessStatusCode)
            {
                Console.WriteLine($"No se encontró ningún amigo con ID {id}.");
                return;
            }

            Amigo amigo = responseGet.Content.ReadAsAsync<Amigo>().Result;
            Console.WriteLine($"Amigo encontrado: {amigo.name}");

            Console.Write("¿Seguro que quieres borrarlo? (s/n): ");
            string? confirmacion = Console.ReadLine();

            if (confirmacion?.ToLower() != "s")
            {
                Console.WriteLine("Borrado cancelado.");
                return;
            }

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
            ConfigureClient();

            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\n===== MENÚ AMIGOS =====");
                Console.WriteLine("1. Listar amigos");
                Console.WriteLine("2. Ver amigo por ID");
                Console.WriteLine("3. Crear amigo");
                Console.WriteLine("4. Modificar amigo");
                Console.WriteLine("5. Borrar amigo");
                Console.WriteLine("0. Salir");
                Console.Write("Selecciona una opción: ");

                string? opcion = Console.ReadLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            ShowFriendList();
                            break;
                        case "2":
                            ShowFriendById();
                            break;
                        case "3":
                            CreateFriend();
                            break;
                        case "4":
                            ModifyFriendInteractive();
                            break;
                        case "5":
                            DeleteFriendInteractive();
                            break;
                        case "0":
                            salir = true;
                            break;
                        default:
                            Console.WriteLine("Opción no válida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Excepción durante la ejecución: {ex.Message}");
                }
            }
        }
    }
}
