namespace Kontur.Tracing.Core.TraceCases.Edi
{
    public static class EdiTraceCases
    {
        public static void AsyncTaskOnServer()
        {
            var synchronizer = new Synchronizer();
            var queue = new RemoteTaskQueue();
            using (new HandlerManager(queue, synchronizer))
            {
                var server = new Server(queue, synchronizer);
                var client = new Client(synchronizer, server);
                client.SendMessage("Task message");
            }
        }
    }
}