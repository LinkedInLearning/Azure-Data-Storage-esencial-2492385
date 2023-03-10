using Azure.Data.Tables;

var storageUri = "[EL URI DE LA CUENTA DE ALMACENAMIENTO]";
var accountName = "[EL NOMBRE DE LA CUENTA DE ALMACENAMIENTO]";
var storageAccountKey = "[LA CLAVE DE LA CUENTA DE ALMACENAMIENTO]";

var serviceClient = new TableServiceClient(new Uri(storageUri),
    new TableSharedKeyCredential(accountName, storageAccountKey));
var table = serviceClient.GetTableClient("mascotas");

ShowMainMenu(table);

static void ShowMainMenu(TableClient table)
{
    Console.WriteLine("Presione la opción deseada:");
    Console.WriteLine("(1) Mostrar todas las entidades");
    Console.WriteLine("(2) Crear una nueva entidad");
    Console.WriteLine("(3) Salir");

    if (!int.TryParse(Console.ReadLine(), out int option))
    {
        return;
    }

    switch (option)
    {
        case 1:
            ShowPets(table);
            break;
        case 2:
            CreatePet(table);
            break;
        default:
            return;
    }
}

static void ShowPets(TableClient table)
{
    var results = table.Query<TableEntity>();
    foreach (var result in results)
    {
        Console.WriteLine($"{result.RowKey}-{result.GetString("nombre")}");
    }
    Console.WriteLine();
    ShowMainMenu(table);
}

static void CreatePet(TableClient table)
{
    Console.WriteLine("Escribe el nombre de la mascota:");
    var petName = Console.ReadLine();
    
    var entity = new TableEntity("pets", Guid.NewGuid().ToString());
    entity["nombre"] = petName;
    table.AddEntity(entity);
    Console.WriteLine("La mascota ha sido creada.");
    Console.WriteLine();
    ShowMainMenu(table);
}