﻿using System;

namespace WorldDirect.CoAP.Example.Client
{
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Net;
    using Util;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var parser = new Parser(with =>
            {
                with.AutoHelp = true;
                with.AutoVersion = true;
                with.HelpWriter = Console.Out;
            });

            var header = new CoapHeader(Type.NonConfirmable, (TokenLength)2, RequestCode.Get, (MessageId)5);
            var tokens = new List<Token>() {(Token)0x01, (Token)0x02,};
            var options = new List<Option>
            {
                (Option)new byte[] {0x01},
            };
            var payload = Encoding.UTF8.GetBytes("Payload");
            var request = CoapRequestMessage.Create(header, tokens, options, payload);
            var client = new WorldDirect.CoAP.Net.CoapClient(new IPEndPoint(IPAddress.Any, 5000));
            await client.SendAsync(request, CancellationToken.None).ConfigureAwait(false);

            var arguments = parser.ParseArguments<GetArguments, PostArguments, DiscoverArguments, ObserverArguments, DeleteArguments>(args);

            arguments.WithParsed<DeleteArguments>(a =>
            {
                var request = Request.NewDelete();
                request.URI = new Uri(a.Endpoint);
                request.Send();

                do
                {
                    Console.WriteLine("Receiving response...");

                    Response response = null;
                    response = request.WaitForResponse();

                    if (response == null)
                    {
                        Console.WriteLine("Request timeout");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Utils.ToString(response));
                        Console.WriteLine("Time elapsed (ms): " + response.RTT);

                        if (response.ContentType == MediaType.ApplicationLinkFormat)
                        {
                            IEnumerable<WebLink> links = LinkFormat.Parse(response.PayloadString);
                            if (links == null)
                            {
                                Console.WriteLine("Failed parsing link format");
                                Environment.Exit(1);
                            }
                            else
                            {
                                Console.WriteLine("Discovered resources:");
                                foreach (var link in links)
                                {
                                    Console.WriteLine(link);
                                }
                            }
                        }
                    }
                } while (false);
            });

            await arguments.WithParsedAsync<PostArguments>(async a =>
            {
                var request = Request.NewPost();
                byte[] payload;
                int mediaType;
                if (Path.HasExtension(a.Payload))
                {
                    payload = await File.ReadAllBytesAsync(a.Payload).ConfigureAwait(false);
                    var fileInfo = new FileInfo(a.Payload);
                    var extension = fileInfo.Name.Split('.')[1];
                    var filename = fileInfo.Name;
                    payload = payload.Concat(Encoding.UTF8.GetBytes($"--{filename}--")).ToArray();
                    mediaType = MediaType.Parse(extension);
                }
                else
                {
                    payload = Encoding.UTF8.GetBytes(a.Payload);
                    mediaType = MediaType.TextPlain;
                }

                request.URI = new Uri(a.Endpoint);
                request.SetPayload(payload, mediaType);

                request.Send();

                do
                {
                    Console.WriteLine("Receiving response...");

                    Response response = null;
                    response = request.WaitForResponse();

                    if (response == null)
                    {
                        Console.WriteLine("Request timeout");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Utils.ToString(response));
                        Console.WriteLine("Time elapsed (ms): " + response.RTT);

                        if (response.ContentType == MediaType.ApplicationLinkFormat)
                        {
                            IEnumerable<WebLink> links = LinkFormat.Parse(response.PayloadString);
                            if (links == null)
                            {
                                Console.WriteLine("Failed parsing link format");
                                Environment.Exit(1);
                            }
                            else
                            {
                                Console.WriteLine("Discovered resources:");
                                foreach (var link in links)
                                {
                                    Console.WriteLine(link);
                                }
                            }
                        }
                    }
                } while (false);
            }).ConfigureAwait(false);

            arguments.WithParsed<DiscoverArguments>(a =>
            {
                var baseUrl = new Uri(a.Endpoint);
                var request = Request.NewGet();
                request.URI = new Uri(baseUrl, ".well-known/core");

                request.Send();

                do
                {
                    Console.WriteLine("Receiving response...");

                    Response response = null;
                    response = request.WaitForResponse();

                    if (response == null)
                    {
                        Console.WriteLine("Request timeout");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Utils.ToString(response));
                        Console.WriteLine("Time elapsed (ms): " + response.RTT);

                        if (response.ContentType == MediaType.ApplicationLinkFormat)
                        {
                            IEnumerable<WebLink> links = LinkFormat.Parse(response.PayloadString);
                            if (links == null)
                            {
                                Console.WriteLine("Failed parsing link format");
                                Environment.Exit(1);
                            }
                            else
                            {
                                Console.WriteLine("Discovered resources:");
                                foreach (var link in links)
                                {
                                    Console.WriteLine(link);
                                }
                            }
                        }
                    }
                } while (false);
            });

            arguments.WithParsed<ObserverArguments>(a =>
            {
                var request = Request.NewGet();
                request.URI = new Uri(a.Endpoint);
                request.MarkObserve();

                request.Send();

                do
                {
                    Console.WriteLine("Receiving response...");

                    Response response = null;
                    response = request.WaitForResponse();

                    if (response == null)
                    {
                        Console.WriteLine("Request timeout");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Utils.ToString(response));
                        Console.WriteLine("Time elapsed (ms): " + response.RTT);
                        break;
                    }
                } while (true);

                request.Cancel();
            });

            arguments.WithParsed<GetArguments>(a =>
            {
                var request = Request.NewGet();
                request.URI = new Uri(a.Endpoint);

                request.Send();
                do
                {
                    Console.WriteLine("Receiving response...");

                    Response response = null;
                    response = request.WaitForResponse();

                    if (response == null)
                    {
                        Console.WriteLine("Request timeout");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Utils.ToString(response));
                        Console.WriteLine("Time elapsed (ms): " + response.RTT);

                        if (response.ContentType == MediaType.ApplicationLinkFormat)
                        {
                            IEnumerable<WebLink> links = LinkFormat.Parse(response.PayloadString);
                            if (links == null)
                            {
                                Console.WriteLine("Failed parsing link format");
                                Environment.Exit(1);
                            }
                            else
                            {
                                Console.WriteLine("Discovered resources:");
                                foreach (var link in links)
                                {
                                    Console.WriteLine(link);
                                }
                            }
                        }
                    }
                } while (a.Looping);
            });

            Console.WriteLine("Fin");
            Console.ReadLine();
        }
    }

    //// .NET 2, .NET 4 entry point
    //class ExampleClient
    //{
    //    public static void Main(String[] args)
    //    {
    //        String method = null;
    //        Uri uri = null;
    //        String payload = null;
    //        Boolean loop = false;
    //        Boolean byEvent = true;

    //        if (args.Length == 0)
    //            PrintUsage();

    //        Int32 index = 0;
    //        foreach (String arg in args)
    //        {
    //            if (arg[0] == '-')
    //            {
    //                if (arg.Equals("-l"))
    //                    loop = true;
    //                if (arg.Equals("-e"))
    //                    byEvent = true;
    //                else
    //                    Console.WriteLine("Unknown option: " + arg);
    //            }
    //            else
    //            {
    //                switch (index)
    //                {
    //                    case 0:
    //                        method = arg.ToUpper();
    //                        break;
    //                    case 1:
    //                        try
    //                        {
    //                            uri = new Uri(arg);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            Console.WriteLine("Failed parsing URI: " + ex.Message);
    //                            Environment.Exit(1);
    //                        }
    //                        break;
    //                    case 2:
    //                        payload = arg;
    //                        break;
    //                    default:
    //                        Console.WriteLine("Unexpected argument: " + arg);
    //                        break;
    //                }
    //                index++;
    //            }
    //        }

    //        if (method == null || uri == null)
    //            PrintUsage();

    //        Request request = NewRequest(method);
    //        if (request == null)
    //        {
    //            Console.WriteLine("Unknown method: " + method);
    //            Environment.Exit(1);
    //        }

    //        if ("OBSERVE".Equals(method))
    //        {
    //            request.MarkObserve();
    //            loop = true;
    //        }
    //        else if ("DISCOVER".Equals(method) &&
    //            (String.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/")))
    //        {
    //            uri = new Uri(uri, "/.well-known/core");
    //        }

    //        request.URI = uri;
    //        request.SetPayload(payload, MediaType.TextPlain);

    //        // uncomment the next line if you want to specify a draft to use
    //        // request.EndPoint = CoAP.Net.EndPointManager.Draft13;

    //        Console.WriteLine(Utils.ToString(request));

    //        try
    //        {
    //            if (byEvent)
    //            {
    //                request.Respond += delegate (Object sender, ResponseEventArgs e)
    //                {
    //                    Response response = e.Response;
    //                    if (response == null)
    //                    {
    //                        Console.WriteLine("Request timeout");
    //                    }
    //                    else
    //                    {
    //                        Console.WriteLine(Utils.ToString(response));
    //                        Console.WriteLine("Time (ms): " + response.RTT);
    //                    }
    //                    if (!loop)
    //                        Environment.Exit(0);
    //                };
    //                request.Send();
    //                while (true)
    //                {
    //                    Console.ReadKey();
    //                }
    //            }
    //            else
    //            {
    //                // uncomment the next line if you need retransmission disabled.
    //                // request.AckTimeout = -1;

    //                request.Send();

    //                do
    //                {
    //                    Console.WriteLine("Receiving response...");

    //                    Response response = null;
    //                    response = request.WaitForResponse();

    //                    if (response == null)
    //                    {
    //                        Console.WriteLine("Request timeout");
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        Console.WriteLine(Utils.ToString(response));
    //                        Console.WriteLine("Time elapsed (ms): " + response.RTT);

    //                        if (response.ContentType == MediaType.ApplicationLinkFormat)
    //                        {
    //                            IEnumerable<WebLink> links = LinkFormat.Parse(response.PayloadString);
    //                            if (links == null)
    //                            {
    //                                Console.WriteLine("Failed parsing link format");
    //                                Environment.Exit(1);
    //                            }
    //                            else
    //                            {
    //                                Console.WriteLine("Discovered resources:");
    //                                foreach (var link in links)
    //                                {
    //                                    Console.WriteLine(link);
    //                                }
    //                            }
    //                        }
    //                    }
    //                } while (loop);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("Failed executing request: " + ex.Message);
    //            Console.WriteLine(ex);
    //            Environment.Exit(1);
    //        }
    //    }

    //    private static Request NewRequest(String method)
    //    {
    //        switch (method)
    //        {
    //            case "POST":
    //                return Request.NewPost();
    //            case "PUT":
    //                return Request.NewPut();
    //            case "DELETE":
    //                return Request.NewDelete();
    //            case "GET":
    //            case "DISCOVER":
    //            case "OBSERVE":
    //                return Request.NewGet();
    //            default:
    //                return null;
    //        }
    //    }

    //    private static void PrintUsage()
    //    {
    //        Console.WriteLine("CoAP.NET Example Client");
    //        Console.WriteLine();
    //        Console.WriteLine("Usage: CoAPClient [-e] [-l] method uri [payload]");
    //        Console.WriteLine("  method  : { GET, POST, PUT, DELETE, DISCOVER, OBSERVE }");
    //        Console.WriteLine("  uri     : The CoAP URI of the remote endpoint or resource.");
    //        Console.WriteLine("  payload : The data to send with the request.");
    //        Console.WriteLine("Options:");
    //        Console.WriteLine("  -e      : Receives responses by the Responded event.");
    //        Console.WriteLine("  -l      : Loops for multiple responses.");
    //        Console.WriteLine("            (automatic for OBSERVE and separate responses)");
    //        Console.WriteLine();
    //        Console.WriteLine("Examples:");
    //        Console.WriteLine("  CoAPClient DISCOVER coap://localhost");
    //        Console.WriteLine("  CoAPClient POST coap://localhost/storage data");
    //        Environment.Exit(0);
    //    }
    //}
}
