namespace WorldDirect.CoAP.Net
{
    partial class EndPointManager
    {
        private static IEndPoint _default;

        private static IEndPoint GetDefaultEndPoint()
        {
            if (_default == null)
            {
                lock (typeof(EndPointManager))
                {
                    if (_default == null)
                    {
                        _default = CreateEndPoint();
                    }
                }
            }
            return _default;
        }

        private static IEndPoint CreateEndPoint()
        {
            CoAPEndPoint ep = new CoAPEndPoint(0);
            ep.Start();
            return ep;
        }
    }
}