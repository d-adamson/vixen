﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Hardware;

namespace VixenTestbed {
	public partial class PatchingForm : Form {
		public PatchingForm() {
			InitializeComponent();
		}

		private void PatchingForm_Load(object sender, EventArgs e) {
			_LoadChannels();
			_LoadControllers();
			_LoadPatching();

			treeViewPatching.ExpandAll();
			treeViewPatching.TopNode = treeViewPatching.Nodes[0];
		}

		private void comboBoxChannel_SelectedIndexChanged(object sender, EventArgs e) {
			_ValidatePatchComponents();
		}

		private void comboBoxController_SelectedIndexChanged(object sender, EventArgs e) {
			_ValidatePatchComponents();
			comboBoxOutput.BeginUpdate();
			comboBoxOutput.Items.Clear();
			if(_SelectedController != null) {
				comboBoxOutput.Items.AddRange(Enumerable.Range(0, _SelectedController.OutputCount).Cast<object>().ToArray());
			}
			comboBoxOutput.EndUpdate();
		}

		private void comboBoxOutput_SelectedIndexChanged(object sender, EventArgs e) {
			_ValidatePatchComponents();
		}

		private void treeViewPatching_AfterSelect(object sender, TreeViewEventArgs e) {
			_ValidatePatchComponents();
		}

		private void buttonPatch_Click(object sender, EventArgs e) {
			_SelectedChannel.Patch.Add(_SelectedController.Id, _SelectedOutput);
			_AddChannelPatchNode(_SelectedChannel, new ControllerReference(_SelectedController.Id, _SelectedOutput));
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			_SelectedPatchChannel.Patch.Remove(_SelectedPatch);
			treeViewPatching.Nodes.Remove(treeViewPatching.SelectedNode);
		}

		private Channel _SelectedChannel {
			get { return comboBoxChannel.SelectedItem as Channel; }
			set { }
		}

		private OutputController _SelectedController {
			get { return comboBoxController.SelectedItem as OutputController; }
			set { }
		}

		private int _SelectedOutput {
			get { return comboBoxOutput.SelectedIndex; }
			set { }
		}

		private ControllerReference _SelectedPatch {
			get {
				if(treeViewPatching.SelectedNode != null) {
					return treeViewPatching.SelectedNode.Tag as ControllerReference;
				}
				return null;
			}
		}

		private Channel _SelectedPatchChannel {
			get {
				if(treeViewPatching.SelectedNode != null && treeViewPatching.SelectedNode.Parent != null) {
					return treeViewPatching.SelectedNode.Parent.Tag as Channel;
				}
				return null;
			}
		}

		private void _ValidatePatchComponents() {
			buttonPatch.Enabled = _SelectedChannel != null && _SelectedController != null && _SelectedOutput != -1;
			buttonRemove.Enabled = _SelectedPatch != null;
		}

		private void _LoadChannels() {
			comboBoxChannel.DisplayMember = "Name";
			comboBoxChannel.ValueMember = "Id";
			comboBoxChannel.DataSource = Vixen.Sys.Execution.Channels.ToArray();
		}

		private void _LoadControllers() {
			comboBoxController.DisplayMember = "Name";
			comboBoxController.ValueMember = "Id";
			comboBoxController.DataSource = OutputController.GetAll().ToArray();
		}

		private void _LoadPatching() {
			treeViewPatching.Nodes.Clear();
			foreach(Channel channel in comboBoxChannel.Items) {
				_AddChannelNode(channel);
			}
		}

		private void _AddChannelNode(Channel channel) {
			TreeNode channelNode = treeViewPatching.Nodes.Add(channel.Name);
			channelNode.Tag = channel;
			foreach(ControllerReference controllerReference in channel.Patch) {
				_AddChannelPatchNode(channelNode, controllerReference);
			}
		}

		private void _AddChannelPatchNode(TreeNode channelNode, ControllerReference controllerReference) {
			TreeNode patchNode = channelNode.Nodes.Add(controllerReference.ToString(true));
			patchNode.Tag = controllerReference;
		}

		private void _AddChannelPatchNode(Channel channel, ControllerReference controllerReference) {
			TreeNode channelNode = treeViewPatching.Nodes.Cast<TreeNode>().FirstOrDefault(x => x.Tag == channel);
			if(channelNode != null) {
				_AddChannelPatchNode(channelNode, controllerReference);
			} else {
				_AddChannelNode(channel);
			}
		}
	}
}