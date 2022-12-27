using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Optional;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace svc
{
	public class Settings
	{
		public LogLevel LogLevel { get; set; } = LogLevel.Warning;


	}

	public enum State
	{
		Invalid,
		Created,
		Starting,
		Running,
		Stopping,
		Finished,
		Error,
	}

	public enum Result
	{
		Invalid,
		Success,
		Failure,
		NotOverriden,
	}



	public class Service<T, TSETTING> : BackgroundService where TSETTING : Settings
	{
		public IOptions<TSETTING> Settings { get; set; }
		public ILogger<T> Log { get; set; }

		public State State { get; protected set; } = State.Invalid;

		protected readonly string Name = "_Service_Name_Unset";

		public Service(ILogger<T> logger, IOptions<TSETTING> settings)
		{
			Log = logger;

			Settings = settings;

			Type t = typeof(T);

			Name = t.Name;

			Log.LogInformation($"Service of type {Name} being created at: {DateTimeOffset.Now}" );

			State = State.Created;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			Log.LogInformation($"{Name} Starting...");
			State = State.Starting;
			var resStart = await Start(stoppingToken);
			Log.LogInformation($"{Name} Start returned {resStart}");

			Log.LogInformation($"{Name} Running...");
			State = State.Running;
			var resRun = await Run(stoppingToken);
			Log.LogInformation($"{Name} Run returned {resStart}");

			Log.LogInformation($"{Name} Stopping...");
			State = State.Stopping;
			var resStop = await Stop(stoppingToken);
			Log.LogInformation($"{Name} Stop returned {resStart}");

			State = State.Finished;
		}

		protected virtual async Task<Result> Start(CancellationToken stoppingToken)
		{
			await Task.Delay(0, stoppingToken);
			return Result.NotOverriden;
		}

		protected virtual async Task<Result> Run(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				Log.LogTrace($"{Name} Tick");
				await Task.Delay(1000, stoppingToken);
			}
			return Result.NotOverriden;
		}

		protected virtual async Task<Result> Stop(CancellationToken stoppingToken)
		{
			await Task.Delay(0, stoppingToken);
			return Result.NotOverriden;
		}
	}


}
