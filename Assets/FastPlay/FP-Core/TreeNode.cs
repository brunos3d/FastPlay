using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay {
	public class TreeNode<T> {

		public GUIContent content;

		public T data;

		public TreeNode<T> parent;

		private ICollection<TreeNode<T>> children_index;

		private Dictionary<string, TreeNode<T>> children;

		public bool isRoot {
			get { return parent == null; }
		}

		public bool isLeaf {
			get { return children.Count == 0; }
		}

		public int level {
			get {
				if (this.isRoot) {
					return 0;
				}
				return parent.level + 1;
			}
		}

		public string path {
			get {
				if (this.isRoot) {
					return content.text;
				}
				return parent.path + "/" + content.text;
			}
		}

		public int Count {
			get {
				return children.Count;
			}
		}

		public TreeNode<T> this[int index] {
			get {
				return children.ElementAt(index).Value;
			}
		}

		public TreeNode(GUIContent content, T data) {
			this.content = content;
			this.data = data;
			this.children = new Dictionary<string, TreeNode<T>>();
			this.children_index = new LinkedList<TreeNode<T>>();
		}

		public void AddAnExistingTreeNode(TreeNode<T> tree) {
			this.children[tree.path] = tree;
			this.RegisterChildForSearch(tree);
		}

		public TreeNode<T> AddChild(GUIContent content, T child) {
			TreeNode<T> child_node = new TreeNode<T>(content, child) { parent = this };
			this.children[content.text] = child_node;
			this.RegisterChildForSearch(child_node);
			return child_node;
		}

		public TreeNode<T> AddChildByPath(GUIContent content, T data) {
			string path = content.text;
			var paths = path.Split('/').Where(s => !s.IsNullOrWhiteSpace());
			string root_path = paths.ElementAt(0);
			int path_count = paths.Count();
			//string end_path = paths[paths.Count - 1];
			if (path_count == 1 && root_path == paths.ElementAt(path_count - 1)) {
				return this.children[root_path] = AddChild(content, data);
			}
			else {
				//1/2/3
				//1/2/3/4/5
				string subpath = path.Replace(string.Format("{0}/", root_path), string.Empty);
				GUIContent subcontent = new GUIContent(subpath, content.image, content.tooltip);
				TreeNode<T> t;
				if (children.TryGetValue(root_path, out t)) {
					return t.AddChildByPath(subcontent, data);
				}
				else {
					TreeNode<T> instance = this.children[root_path] = AddChild(new GUIContent(root_path), default(T));
					return instance.AddChildByPath(subcontent, data);
				}
			}
		}

		private void RegisterChildForSearch(TreeNode<T> node) {
			children_index.Add(node);
			if (parent != null) {
				parent.RegisterChildForSearch(node);
			}
		}

		public TreeNode<T> FindTreeNode(Func<TreeNode<T>, bool> predicate) {
			return this.children_index.FirstOrDefault(predicate);
		}

		public TreeNode<T> GetTreeNodeInChildren(Func<TreeNode<T>, bool> predicate) {
			TreeNode<T> search_tree = new TreeNode<T>(new GUIContent("Search"), default(T));
			foreach (TreeNode<T> node in this.children_index.Where(predicate)) {
				search_tree.AddAnExistingTreeNode(node);
			}
			return search_tree;
		}
	}
}
