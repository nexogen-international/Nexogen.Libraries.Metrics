using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using Xunit;
using Nexogen.Libraries.Metrics.Prometheus.AspCore;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Nexogen.Libraries.Metrics.Prometheus.AspCoreTests
{
    public class MetricsPathTests
    {
        [Fact]
        public async Task LiteralPathsAreHandledCorrectly()
        {
            var capture = new PathCapture();
            var builder = new WebHostBuilder()
                .ConfigureServices(svc => svc.AddPrometheus())
                .Configure(app =>
                {
                    app.UsePrometheus(Ext.GetAndCapturePath(capture));
                    app.Run(async ctx => await ctx.Response.WriteAsync("Hello world"));
                });
            using (var server = new TestServer(builder))
            using (var client = server.CreateClient())
            {
                await client.GetAsync("/test");
                await client.GetAsync("/something_else");
            }
            Assert.Equal("/test", capture.Paths[0]);
            Assert.Equal("/something_else", capture.Paths[1]);
        }

        [Fact]
        public async Task MappedPathsAreHandledCorrectly()
        {
            var capture = new PathCapture();
            var builder = new WebHostBuilder()
                .ConfigureServices(svc => svc.AddPrometheus())
                .Configure(app =>
                {
                    app.UsePrometheus(Ext.GetAndCapturePath(capture));
                    app.Map("/user", cfg => cfg.Run(async ctx => await ctx.Response.WriteAsync("Hello world")));
                    app.Map("/agent", cfg => cfg.Run(async ctx => await ctx.Response.WriteAsync("Hello world")));
                });
            using (var server = new TestServer(builder))
            using (var client = server.CreateClient())
            {
                await client.GetAsync("/user");
                await client.GetAsync("/agent");
            }
            Assert.Equal("/user", capture.Paths[0]);
            Assert.Equal("/agent", capture.Paths[1]);
        }

        [Fact]
        public async Task RoutingWithoutTemplateHandledCorrectly()
        {
            var capture = new PathCapture();
            var builder = new WebHostBuilder()
                .ConfigureServices(svc => svc.AddPrometheus().AddRouting())
                .Configure(app =>
                {
                    app.UsePrometheus(Ext.GetAndCapturePath(capture));
                    var routes = new RouteBuilder(app);
                    routes.MapGet("route1", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    routes.MapGet("route2", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    app.UseRouter(routes.Build());
                });
            using (var server = new TestServer(builder))
            using (var client = server.CreateClient())
            {
                await client.GetAsync("/route1");
                await client.GetAsync("/route2");
            }
            Assert.Equal("/route1", capture.Paths[0]);
            Assert.Equal("/route2", capture.Paths[1]);
        }

        [Fact]
        public async Task RoutingWithTemplatesHandledCorrectly()
        {
            var capture = new PathCapture();
            var builder = new WebHostBuilder()
                .ConfigureServices(svc => svc.AddPrometheus().AddRouting())
                .Configure(app =>
                {
                    app.UsePrometheus(Ext.GetAndCapturePath(capture));
                    var routes = new RouteBuilder(app);
                    routes.MapGet("route1/{userid:int}", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    routes.MapGet("route2/{agentid:guid}", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    app.UseRouter(routes.Build());
                });
            using (var server = new TestServer(builder))
            using (var client = server.CreateClient())
            {
                await client.GetAsync("/route1/4");
                await client.GetAsync($"/route2/{Guid.Empty}");
            }
            Assert.Equal("/route1/{userid:int}", capture.Paths[0]);
            Assert.Equal("/route2/{agentid:guid}", capture.Paths[1]);
        }

        [Fact]
        public async Task MappedRoutingWithTemplatesHandledCorrectly()
        {
            var capture = new PathCapture();
            var builder = new WebHostBuilder()
                .ConfigureServices(svc => svc.AddPrometheus().AddRouting())
                .Configure(app =>
                {
                    app.UsePrometheus(Ext.GetAndCapturePath(capture));
                    var routes = new RouteBuilder(app);
                    routes.MapGet("route1/{userid:int}", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    routes.MapGet("route2/{agentid:guid}", async ctx => await ctx.Response.WriteAsync("Hello world"));
                    app.Map("/nested", nested => nested.UseRouter(routes.Build()));
                });
            using (var server = new TestServer(builder))
            using (var client = server.CreateClient())
            {
                await client.GetAsync("/nested/route1/4");
                await client.GetAsync($"/nested/route2/{Guid.Empty}");
            }
            Assert.Equal("/nested/route1/{userid:int}", capture.Paths[0]);
            Assert.Equal("/nested/route2/{agentid:guid}", capture.Paths[1]);
        }
    }
}
