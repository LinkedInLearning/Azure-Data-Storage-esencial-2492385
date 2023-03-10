using Azure.Storage.Queues;
var connectionString = "[TU CADENA DE CONEXION]";

var queueClient = new QueueClient(connectionString, "mensajes");
queueClient.SendMessage("Hola LinkedIn Learning");
Console.WriteLine("El mensaje ha sido enviado.");

var messages = queueClient.ReceiveMessages(10).Value;
foreach (var message in messages)
{
    Console.WriteLine(message.Body);
    queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
}