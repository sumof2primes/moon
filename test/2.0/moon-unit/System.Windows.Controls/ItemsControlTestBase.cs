using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Moonlight.UnitTesting;
using System.Collections;

namespace MoonTest.System.Windows.Controls
{
	public interface IPoker
	{
		// This item will be returned by the next call to GetContainerItem
		DependencyObject ContainerItem { get; set; }

		DependencyObject LastClearedContainer { get; }
		DependencyObject LastCreatedContainer { get; }
		DependencyObject LastPreparedContainer { get; }
		object LastPreparedItem { get; }

		bool ApplyTemplate ();
		void ClearContainerForItemOverride_ (DependencyObject container, object item);
		DependencyObject GetContainerForItemOverride_ ();
		bool IsItemItsOwnContainerOverride_ (object item);
		void PrepareContainerForItemOverride_ (DependencyObject container, object item);

		string DisplayMemberPath { get; set; }
		ItemCollection Items { get; }
		Style ItemContainerStyle { get; set; }
		IEnumerable ItemsSource { get; set; }
		DataTemplate ItemTemplate { get; set; }
		object SelectedItem { get; set; }
		int SelectedIndex { get; set;  }
		Style Style { get; set; }
	}

	public abstract class ItemsControlTestBase : SilverlightTest
	{
		IPoker currentControl;
		public IPoker CurrentControl {
			get
			{
				if (currentControl == null)
					currentControl = CreateControl ();
				return currentControl;
			}
		}

		protected abstract IPoker CreateControl ();
		protected abstract object CreateContainer ();

		[TestCleanup]
		public void Cleanup ()
		{
			currentControl = null;
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ (null, null);
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride2 ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ (null, new object ());
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride3 ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ ((DependencyObject) CurrentControl, null);
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride4 ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ (new ContentPresenter (), new object ());
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride5 ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ (new ListBoxItem (), new object ());
		}

		[TestMethod]
		public virtual void ClearContainerForItemOverride6 ()
		{
			IPoker p = CurrentControl;
			p.ClearContainerForItemOverride_ (new ComboBoxItem (), new object ());
		}

		[TestMethod]
		[Asynchronous]
		public virtual void ContainerItemTest ()
		{
			IPoker box = CurrentControl;
			var item1 = new ContentPresenter ();
			var item2 = new ListBoxItem ();
			var item3 = new ComboBoxItem ();

			CreateAsyncTest ((FrameworkElement) box,
				() => box.ApplyTemplate (),
				() => {
					box.Items.Add (item1);
					box.Items.Add (item2);
					box.Items.Add (item3);
				},
				() => {
					Assert.IsNull (item1.DataContext);
					Assert.IsNull (item2.DataContext);
					Assert.IsNull (item3.DataContext);
				}
			);
		}

		[TestMethod]
		[Asynchronous]
		public virtual void ContainerItemTest2 ()
		{
			object item = new object ();
			IPoker c = CurrentControl;

			TestPanel.Children.Add ((FrameworkElement) c);
			Enqueue (() => c.ApplyTemplate ());
			Enqueue (() => c.Items.Add (item));
		}

		class ConceteElement : FrameworkElement { }

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest5 ()
		{
			ConceteElement item = new ConceteElement ();
			ItemsControlPoker c = new ItemsControlPoker ();
			c.ApplyTemplate ();
			CreateAsyncTest (c, () => {
				c.Items.Add (item);
				Assert.IsNull (c.LastCreatedContainer, "#1"); // No autogenerated container
				Assert.IsNull (item.DataContext, "#3");
			});
		}

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest6 ()
		{
			object item = new object ();
			ConceteElement container = new ConceteElement ();
			ItemsControlPoker c = new ItemsControlPoker ();
			c.ContainerItem = container;
			c.ApplyTemplate ();
			CreateAsyncTest (c, () => {
				c.Items.Add (item);
				Assert.AreEqual (container, c.LastCreatedContainer, "#1");
				Assert.AreEqual (container.DataContext, item, "#2");
				Assert.AreEqual (container.ReadLocalValue (FrameworkElement.DataContextProperty), item, "#3");
			});
		}

		[TestMethod]
		[Asynchronous]
		public void ContainerItemTest7 ()
		{
			// Force all elements to *not* be their own container
			ItemsControlPoker c = new ItemsControlPoker { IsOwnContainer = false };
			c.ApplyTemplate ();

			CreateAsyncTest (c, () => {
				ContentPresenter item;
				object content;

				content = new Rectangle ();
				c.Items.Add (content);
				Assert.IsInstanceOfType<ContentPresenter> (c.LastCreatedContainer, "#1");
				item = (ContentPresenter) c.LastCreatedContainer;
				Assert.AreEqual (content, item.Content, "#2");
				Assert.IsNull (item.DataContext, "#3"); // Fails in Silverlight 3
				c.LastCreatedContainer = null;

				content = "I'm a string";
				c.Items.Add (content);
				Assert.IsInstanceOfType<ContentPresenter> (c.LastCreatedContainer, "#4");
				item = (ContentPresenter) c.LastCreatedContainer;
				Assert.AreEqual (content, item.Content, "#5");
				Assert.AreEqual (content, item.DataContext, "#6");
			});
		}

		[TestMethod]
		[Asynchronous]
		public virtual void DisplayMemberPathTest ()
		{
			// Check if 'DisplayMemberPath' is used when a UIElement
			// is added to the ItemsCollection
			ItemsControl c = (ItemsControl)CurrentControl;
			CurrentControl.DisplayMemberPath = "Width";

			TestPanel.Children.Add (c);
			Enqueue (() => c.ItemTemplate = null);
			Enqueue (() => c.ApplyTemplate ());
			Enqueue (() => c.Items.Add (new Rectangle { Width = 10 }));
			// Validation is performed in the subclasses
		}

		[TestMethod]
		[Asynchronous]
		public virtual void DisplayMemberPathTest2 ()
		{
			// Check if 'DisplayMemberPath' is used when a UIElement
			// is added to the ItemsCollection
			ItemsControl c = (ItemsControl) CurrentControl;
			CurrentControl.DisplayMemberPath = "Width";

			TestPanel.Children.Add (c);
			Enqueue (() => c.ItemTemplate = null);
			Enqueue (() => c.ApplyTemplate ());
			Enqueue (() => c.Items.Add (new object ()));
			// Validation is performed in the subclasses
		}

		[TestMethod]
		public virtual void GetContainerForItemOverride ()
		{
			IPoker p = CurrentControl;
			var first = p.GetContainerForItemOverride_ ();
			var second = p.GetContainerForItemOverride_ ();
			Assert.AreNotSame (first, second, "ReferenceEquals");
		}

		[TestMethod]
		public virtual void GetContainerForItemOverride2 ()
		{
			// Subclass must do the testing of the created item
			CurrentControl.GetContainerForItemOverride_ ();

			//poker.ItemContainerStyle = new Style (typeof (ListBoxItem));
			//container = poker.Call_GetContainerForItemOverride ();
			//Assert.AreEqual (poker.ItemContainerStyle, ((ListBoxItem) container).Style, "style applied");
		}

		[TestMethod]
		[Asynchronous]
		public virtual void GetContainerForItemOverride10 ()
		{
			IPoker box = CurrentControl;

			TestPanel.Children.Add ((FrameworkElement) box);
			Enqueue (() => box.ContainerItem = new ListBoxItem ());
			Enqueue (() => box.ApplyTemplate ());
		}

		[TestMethod]
		public virtual void IsItemItsOwnContainerTest ()
		{
			ItemsControlPoker ic = new ItemsControlPoker ();
			Assert.IsFalse (ic.IsItemItsOwnContainerOverride_ (null), "null");
			Assert.IsFalse (ic.IsItemItsOwnContainerOverride_ (new OpenFileDialog ()), "OpenFileDialog");
			Assert.IsFalse (ic.IsItemItsOwnContainerOverride_ (ic.Items), "ItemCollection");
			Assert.IsFalse (ic.IsItemItsOwnContainerOverride_ (new RowDefinition ()), "RowDefinition");
		}

		[TestMethod]
		[Asynchronous]
		public virtual void ItemTemplateTest3 ()
		{
			ItemsControl box = (ItemsControl) CurrentControl;

			CurrentControl.ItemTemplate = null;

			TestPanel.Children.Add (box);
			Enqueue (() => CurrentControl.DisplayMemberPath = "Test");
			Enqueue (() => box.ApplyTemplate ());
			Enqueue (() => CurrentControl.Items.Add (new object ()));
			// Subclasses do the validation
		}

		[TestMethod]
		[Asynchronous]
		public virtual void ItemTemplateTest4 ()
		{
			ItemsControl box = (ItemsControl) CurrentControl;

			CurrentControl.ItemTemplate = null;
			CurrentControl.DisplayMemberPath = "Test";
			CurrentControl.Items.Add (new ListBoxItem ());
			CurrentControl.Items.Add (new ComboBoxItem ());

			TestPanel.Children.Add (box);
			EnqueueWaitLoaded (box, "#loaded");
			Enqueue (() => box.ApplyTemplate ());
			Enqueue (() => {
				ListBoxItem item = (ListBoxItem) CurrentControl.Items [0];
				ComboBoxItem item2 = (ComboBoxItem) CurrentControl.Items [1];
				Assert.IsNull (item.ContentTemplate, "#1");
				Assert.IsNull (item2.ContentTemplate, "#2");
			});
			EnqueueTestComplete ();
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest ()
		{
			IPoker box = CurrentControl;
			box.PrepareContainerForItemOverride_ (null, null);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest2 ()
		{
			IPoker box = CurrentControl;
			box.PrepareContainerForItemOverride_ (new Rectangle (), null);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest2b ()
		{
			IPoker box = CurrentControl;
			box.PrepareContainerForItemOverride_ ((DependencyObject)box, null);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest3 ()
		{
			IPoker box = CurrentControl;
			ComboBoxItem item = new ComboBoxItem ();
			Assert.IsNull (item.Style);
			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);

			box.PrepareContainerForItemOverride_ (item, item);

			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest3b ()
		{
			IPoker box = CurrentControl;
			ListBoxItem item = new ListBoxItem ();
			Assert.IsNull (item.Style);
			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);

			box.PrepareContainerForItemOverride_ (item, item);

			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest3c ()
		{
			IPoker box = CurrentControl;
			ContentPresenter item = new ContentPresenter ();
			Assert.IsNull (item.Style);
			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);

			box.PrepareContainerForItemOverride_ (item, item);

			Assert.IsNull (item.Content);
			Assert.IsNull (item.ContentTemplate);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest4 ()
		{
			Rectangle rect = new Rectangle ();
			IPoker box = CurrentControl;
			ComboBoxItem item = new ComboBoxItem ();
			Assert.IsNull (item.Content);
			box.PrepareContainerForItemOverride_ (item, rect);
			Assert.AreSame (item.Content, rect);
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest5 ()
		{
			Rectangle rect = new Rectangle ();
			IPoker box = CurrentControl;
			box.Items.Add (rect);
			ComboBoxItem item = new ComboBoxItem ();
			Assert.Throws<InvalidOperationException> (() => box.PrepareContainerForItemOverride_ (item, rect));
		}

		[TestMethod]
		public virtual void PrepareContainerForItemOverrideTest6 ()
		{
			Rectangle rect = new Rectangle ();
			IPoker box = CurrentControl;
			ContentPresenter item = new ContentPresenter ();
			Assert.IsNull (item.Content);
			box.PrepareContainerForItemOverride_ (item, rect);
			Assert.AreSame (item.Content, rect);
		}
	}
}
