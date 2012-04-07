﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vixen.Commands;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Output {
	abstract public class OutputDeviceBase : IOutputDevice {
		private bool _isRunning;
		private OutputDeviceRefreshRateValue _refreshRateValue;
		private OutputDeviceUpdateTimeValue _updateTimeValue;
		private Stopwatch _stopwatch;

		protected OutputDeviceBase(Guid id, string name) {
			Id = id;
			Name = name;
			_stopwatch = new Stopwatch();
		}

		public void Start() {
			if(!IsRunning) {
				_isRunning = true;
				_Start();
				_SetupInstrumentation();
			}
		}
		abstract protected void _Start();

		//public void Pause() {
		//    if(IsRunning) {
		//        _Pause();
		//    }
		//}
		//abstract protected void _Pause();

		//public void Resume() {
		//    if(IsRunning) {
		//        _Resume();
		//    }
		//}
		//abstract protected void _Resume();

		public void Stop() {
			if(IsRunning) {
				_Stop();
				_isRunning = false;
				VixenSystem.Instrumentation.RemoveValue(_refreshRateValue);
				VixenSystem.Instrumentation.RemoveValue(_updateTimeValue);
				_stopwatch.Stop();
			}
		}
		abstract protected void _Stop();

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() {
			return false;
		}

		public Guid Id { get; protected set; }

		virtual public string Name { get; set; }

		public int UpdateInterval { get; set; }

		virtual public bool IsRunning {
			get { return _isRunning; }
		}

		public void Update() {
			_refreshRateValue.Increment();

			// First, get what we pull from to update...
			Execution.UpdateState();
	
			// Then we update ourselves from that.
			_stopwatch.Restart();
			_UpdateState();
			_updateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
		}

		public IDataPolicy DataPolicy { get; set; }

		public ICommand GenerateCommand(IEnumerable<IIntentState> outputState) {
			return DataPolicy.GenerateCommand(outputState);
		}

		abstract protected void _UpdateState();

		private void _SetupInstrumentation() {
			_refreshRateValue = new OutputDeviceRefreshRateValue(this);
			VixenSystem.Instrumentation.AddValue(_refreshRateValue);
			_updateTimeValue = new OutputDeviceUpdateTimeValue(this);
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
			_stopwatch.Reset();
			_stopwatch.Start();
		}
	}
}
