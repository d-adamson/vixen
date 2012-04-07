﻿using System;

namespace Vixen.Sys.Output {
	public interface IOutputDevice : ISetup {
		void Start();
		void Stop();
		Guid Id { get; }
		string Name { get; set; }
		int UpdateInterval { get; set; }
		bool IsRunning { get; }
		void Update();
		IDataPolicy DataPolicy { get; set; }
	}
}
