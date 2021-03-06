﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FolderExaminer
{
	// NOTE FOR ATTRIBUTION:
	// The file icons in imageList1 were not created by me, they were a free download from
	// https://www.vecteezy.com/vector-art/121958-colorful-file-icons
	// designer:
	// <a href="https://www.vecteezy.com/free-vector/pdf">Pdf Vectors by Vecteezy</a>

	public enum FileType
	{
		EXCEL,
		PDF,
		POWERPOINT,
		TEXT,
		WORD,
		FOLDER,
		FILE,
	}

	public partial class Form1 : Form
	{
		private string currentPath = "";

		public Form1()
		{
			InitializeComponent();
		}

		private void findFolderButton_Click(object sender, EventArgs e)
		{
			findFolder();
		}

		private void folderPathTextBox_DoubleClick(object sender, EventArgs e)
		{
			findFolder();
		}

		private void examineFolderButton_Click(object sender, EventArgs e)
		{
			currentPath = folderPathTextBox.Text;

			if(currentPath == "" || !isFolder(currentPath))
			{
				return;
			}

			examineFolder(currentPath);
		}

		private void findFolder()
		{
			if(fbd.ShowDialog() == DialogResult.OK || fbd.ShowDialog() == DialogResult.Yes)
			{
				folderPathTextBox.Text = fbd.SelectedPath;
			}
		}

		private bool isFolder(string path)
		{
			return Directory.Exists(path);
		}

		private string getItemName(string path)
		{
			return path.Substring(path.LastIndexOf('\\') + 1);
		}

		private string getItemExtension(string path)
		{
			return path.Substring(path.LastIndexOf('.') + 1);
		}

		private int getImageIndex(string ext)
		{
			switch(ext)
			{
				case "xlsx":
				case "xls":
				case "xlsm":
					return 0;
				case "pdf":
					return 1;
				case "pptx":
				case "ppt":
					return 2;
				case "txt":
					return 3;
				case "doc":
				case "docx":
					return 4;
				default:
					return 6;
			}
		}

		private void examineFolder(string path)
		{
			treeView1.Nodes.Clear();
			treeView1.BeginUpdate();

			treeView1.Nodes.Add(createTree(path));

			treeView1.EndUpdate();
			treeView1.ExpandAll();
		}

		private TreeNode createTree(string path)
		{
			TreeNode rootNode = new TreeNode(getItemName(path));

			try
			{
				// get folders
				foreach (string s in Directory.GetDirectories(path))
				{
					TreeNode folderNode = createTree(s);
					folderNode.ImageIndex = 0;
					folderNode.SelectedImageIndex = 0;
					rootNode.Nodes.Add(folderNode);
				}

				// get files
				foreach (string s in Directory.GetFiles(path))
				{
					Icon iconForFile = SystemIcons.WinLogo;

					if (!imageList1.Images.ContainsKey(getItemExtension(s)))
					{
						iconForFile = Icon.ExtractAssociatedIcon(s);
						imageList1.Images.Add(getItemExtension(s), iconForFile);
					}

					TreeNode fileNode = new TreeNode(getItemName(s));
					fileNode.SelectedImageIndex = imageList1.Images.IndexOfKey(getItemExtension(s));
					fileNode.ImageIndex = imageList1.Images.IndexOfKey(getItemExtension(s));

					rootNode.Nodes.Add(fileNode);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			return rootNode;
		}
	}

	class Folder
	{
		public string title;

		public List<Folder> folders = new List<Folder>();
		public List<string> files = new List<string>();

		public Folder() : this("")
		{

		}

		public Folder(string title = "")
		{
			this.title = title;
		}
	}
}
