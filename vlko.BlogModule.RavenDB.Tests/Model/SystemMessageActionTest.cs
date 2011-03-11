﻿using System;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Action;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class SystemMessageActionTest : LocalClientTest
	{
		private SystemMessage[] _messages;
		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			DBInit.RegisterDocumentStore(Store);
			DBInit.RegisterIndexes(Store);

			using (var tran = RepositoryFactory.StartTransaction())
			{
				// create items as they can 
				_messages = new[]
				            	{
				            		new SystemMessage
				            			{
											Id = Guid.NewGuid(),
				            				CreatedDate = new DateTime(2010, 10, 1),
				            				Sender = "test",
				            				SystemMessageType = SystemMessageTypeEnum.Urgent,
				            				Text = "test_mesage1"
				            			},
				            		new SystemMessage
				            			{
											Id = Guid.NewGuid(),
				            				CreatedDate = new DateTime(2010, 10, 2),
				            				Sender = "test",
				            				SystemMessageType = SystemMessageTypeEnum.Warning,
				            				Text = "test_mesage2"
				            			},
				            		new SystemMessage
				            			{
											Id = Guid.NewGuid(),
				            				CreatedDate = new DateTime(2010, 10, 3),
				            				Sender = "test",
				            				SystemMessageType = SystemMessageTypeEnum.Error,
				            				Text = "test_mesage3"
				            			}
				            	};
				foreach (var systemMessage in _messages)
				{
					SessionFactory<SystemMessage>.Store(systemMessage);
				}
				tran.Commit();
			}
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		[TestMethod]
		public void Test_get_all_sort_by_date_desc()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<ISystemMessageAction>();

				var items = action.GetAll().OrderByDescending(item => item.CreatedDate).ToArray();

				Assert.AreEqual(_messages.Length, items.Length);
				// compare items to _));
				for (int i = 0; i < items.Length; i++)
				{
					var originalItem = _messages[items.Length - 1 - i];
					var dbItem = items[i];
					Assert.AreEqual(originalItem.CreatedDate, dbItem.CreatedDate);
					Assert.AreEqual(originalItem.Id, dbItem.Id);
					Assert.AreEqual(originalItem.Sender, dbItem.Sender);
					Assert.AreEqual(originalItem.SystemMessageType, dbItem.SystemMessageType);
					Assert.AreEqual(originalItem.Text, dbItem.Text);
				}
			}
		}

		[TestMethod]
		public void Test_add_new()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<ISystemMessageAction>();

				var newItem = new SystemMessage
								{
									CreatedDate = new DateTime(2010, 10, 10),
									Sender = "add_new",
									SystemMessageType = SystemMessageTypeEnum.Error,
									Text = "new_message"
								};
				using (var tran = RepositoryFactory.StartTransaction())
				{
					newItem = action.Create(newItem);
					tran.Commit();
				}
				
				var items = action.GetAll().OrderByDescending(item => item.CreatedDate).ToPage(0, 1);

				Assert.AreEqual(1, items.Length);

				var dbItem = items[0];
				Assert.AreEqual(newItem.CreatedDate, dbItem.CreatedDate);
				Assert.AreEqual(newItem.Id, dbItem.Id);
				Assert.AreEqual(newItem.Sender, dbItem.Sender);
				Assert.AreEqual(newItem.SystemMessageType, dbItem.SystemMessageType);
				Assert.AreEqual(newItem.Text, dbItem.Text);
			}
		}
	}
}
