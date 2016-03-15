namespace Kontur.Tracing.Core.TraceCases.Edi
{
    public static class EdiTraceCases
    {
        public static void AsyncTaskOnServer()
        {
            const string url = "http://localhost:12345/sendmessage/";
            var synchronizer = new Synchronizer();
            var queue = new RemoteTaskQueue();
            using (new HandlerManager(queue, synchronizer))
            using (new Server(url, queue, synchronizer))
            {
                var client = new Client(url, synchronizer);
                client.SendMessage("Task message");
            }
        }
    }
}