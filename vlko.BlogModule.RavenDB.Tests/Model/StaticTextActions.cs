﻿using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class StaticTextActions : LocalClientTest
	{
		private StaticText[] _testData;

		private User _user;

		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			DBInit.RegisterDocumentStore(Store);
			DBInit.RegisterIndexes(Store);

			using (var tran = RepositoryFactory.StartTransaction())
			{
				_user = new User()
							   {
								   Id = Guid.NewGuid(),
								   Name = "user",
								   Email = "user@user.sk"
							   };
				var statText1 = new StaticText
									{
										Id = Guid.NewGuid(),
										Title = "text1",
										FriendlyUrl = "text1",
										CreatedBy = _user,
										CreatedDate = DateTime.Now,
										ActualVersion = 1,
										PublishDate = new DateTime(2002, 1, 1),
										Modified = new DateTime(2002, 3, 1),
										AreCommentAllowed = true,
										StaticTextVersions = new List<StaticTextVersion>
																 {
																	 new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "version0",
																			 Version = 0,
																			 CreatedDate = new DateTime(2002, 1, 1)
																		 },
																	new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "version1",
																			 Version = 1,
																			 CreatedDate = new DateTime(2002, 2, 1)
																		 }
																 }
									};
				 statText1.Comments = new List<Comment>
										 {
											 new Comment
												 {
													 Id = Guid.NewGuid(),
													 Name = "first_comment",
													 Content = statText1,
													 Owner = _user,
													 AnonymousName = null,
													 CreatedDate = new DateTime(2002, 1, 1),
													 ActualVersion = 0,
													 CommentVersions = new List<CommentVersion>()
																		   {
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										1, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.1.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen",
																					   Version = 0,
																				   }
																		   }
												 },
											 new Comment
												 {
													 Id = Guid.NewGuid(),
													 Name = "first_comment",
													 Content = statText1,
													 Owner = _user,
													 AnonymousName = null,
													 CreatedDate = new DateTime(2002, 1, 1),
													 ActualVersion = 1,
													 CommentVersions = new List<CommentVersion>()
																		   {
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										1, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.1.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen",
																					   Version = 0
																				   },
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										2, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.2.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen - change",
																					   Version = 1
																				   }
																		   }
												 }

										 };
				var statText2 = new StaticText
				{
					Id = Guid.NewGuid(),
					Title = "text2",
					FriendlyUrl = "text2",
					CreatedBy = _user,
					CreatedDate = DateTime.Now,
					ActualVersion = 2,
					PublishDate = new DateTime(2002, 2, 1),
					Modified = new DateTime(2002, 3, 1),
					AreCommentAllowed = true,
					StaticTextVersions = new List<StaticTextVersion>
																 {
																	 new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "version0",
																			 Version = 0,
																			 CreatedDate = new DateTime(2002, 1, 1)
																		 },
																	new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "version1",
																			 Version = 1,
																			 CreatedDate = new DateTime(2002, 2, 1)
																		 },
																	new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "version2",
																			 Version = 2,
																			 CreatedDate = new DateTime(2002, 3, 1)
																		 }
																 }
				};
				var statText3 = new StaticText
				{
					Id = Guid.NewGuid(),
					Title = "text3",
					FriendlyUrl = "text",
					CreatedBy = _user,
					CreatedDate = DateTime.Now,
					ActualVersion = 0,
					PublishDate = new DateTime(2002, 3, 1),
					Modified = new DateTime(2002, 3, 1),
					AreCommentAllowed = true,
					StaticTextVersions = new List<StaticTextVersion>
																 {
																	 new StaticTextVersion
																		 {
																			 CreatedBy = _user,
																			 Text = "only one version",
																			 Version = 0,
																			 CreatedDate = new DateTime(2002, 1, 1)
																		 },
																 }
				};

				SessionFactory<User>.Store(_user);
				SessionFactory<StaticText>.Store(statText1);
				SessionFactory<StaticText>.Store(statText2);
				SessionFactory<StaticText>.Store(statText3);
				foreach (var comment in statText1.Comments)
				{
					SessionFactory<Comment>.Store(comment);
				}
				_testData = new[] { statText1, statText2, statText3 };
				tran.Commit();
			}
			
			WaitForIndexing();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		[TestMethod]
		public void Test_find_by_primary_key()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

				// get first
				var first = crudActions.FindByPk(_testData[0].Id);
				Assert.AreEqual((object) _testData[0].Id, first.Id);
				Assert.AreEqual((object) _testData[0].Title, first.Title);
				Assert.AreEqual((object) _testData[0].StaticTextVersions[1].CreatedDate, first.ChangeDate);
				Assert.AreEqual((object) _testData[0].StaticTextVersions[1].Text, first.Text);

				// get second
				var second = crudActions.FindByPk(_testData[1].Id);
				Assert.AreEqual((object) _testData[1].Id, second.Id);
				Assert.AreEqual((object) _testData[1].Title, second.Title);
				Assert.AreEqual((object) _testData[1].StaticTextVersions[2].CreatedDate, second.ChangeDate);
				Assert.AreEqual((object) _testData[1].StaticTextVersions[2].Text, second.Text);

				// get third
				var third = crudActions.FindByPk(_testData[2].Id);
				Assert.AreEqual((object) _testData[2].Id, third.Id);
				Assert.AreEqual((object) _testData[2].Title, third.Title);
				Assert.AreEqual((object) _testData[2].StaticTextVersions[0].CreatedDate, third.ChangeDate);
				Assert.AreEqual((object) _testData[2].StaticTextVersions[0].Text, third.Text);
			}
		}

		[TestMethod]
		public void Test_create()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new StaticTextCRUDModel()
							   {
								   Creator = _user,
								   ChangeDate = new DateTime(2002, 1, 1),
								   PublishDate = new DateTime(2002, 2, 1),
								   AllowComments = true,
								   Title = "čreate new",
								   Description = "description new",
								   FriendlyUrl = "create-new",
								   Text = "create new content"
							   };

				var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				Assert.AreEqual("create-new", item.FriendlyUrl);

				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.FriendlyUrl, storedItem.FriendlyUrl);
				Assert.AreEqual(((User)item.Creator).Id, ((User)storedItem.Creator).Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);
			}
		}

		[TestMethod]
		public void Test_update()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new StaticTextCRUDModel()
				{
					Creator = _user,
					ChangeDate = new DateTime(2002, 1, 1),
					PublishDate = new DateTime(2002, 2, 1),
					AllowComments = true,
					Title = "text",
					FriendlyUrl = "text_update",
					Text = "updateable new content",
					Description = "description update"
				};

				var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				// check created item
				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.FriendlyUrl, storedItem.FriendlyUrl);
				Assert.AreEqual(((User)item.Creator).Id, ((User)storedItem.Creator).Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);

				// change item
				item.Text = "updateable new content - change 1";
				item.Description = "updated description update";
				item.ChangeDate = new DateTime(2002, 2, 1);

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Update(item);
					tran.Commit();
				}

				// check changed item
				storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.FriendlyUrl, storedItem.FriendlyUrl);
				Assert.AreEqual(((User)item.Creator).Id, ((User)storedItem.Creator).Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);

				// change item
				item.Text = "updateable new content - change 2";
				item.ChangeDate = new DateTime(2002, 3, 1);

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Update(item);
					tran.Commit();
				}

				// check changed item
				storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(((User)item.Creator).Id, ((User)storedItem.Creator).Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);

				// check if there are 3 items in history
				var staticText = SessionFactory<StaticText>.Load(item.Id);
				Assert.AreEqual(3, staticText.StaticTextVersions.Count);
				Assert.AreEqual(new DateTime(2002, 1, 1), staticText.StaticTextVersions[0].CreatedDate);
				Assert.AreEqual("updateable new content", staticText.StaticTextVersions[0].Text);
				Assert.AreEqual(new DateTime(2002, 2, 1), staticText.StaticTextVersions[1].CreatedDate);
				Assert.AreEqual("updateable new content - change 1", staticText.StaticTextVersions[1].Text);
				Assert.AreEqual(new DateTime(2002, 3, 1), staticText.StaticTextVersions[2].CreatedDate);
				Assert.AreEqual("updateable new content - change 2", staticText.StaticTextVersions[2].Text);
			}
		}

		[TestMethod]
		public void Test_delete()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				
				var item = new StaticTextCRUDModel()
							   {
								   Creator = _user,
								   ChangeDate = new DateTime(2002, 1, 1),
								   PublishDate = new DateTime(2002, 2, 1),
								   AllowComments = true,
								   Title = "updateable new",
								   Text = "updateable new content"
							   };

				var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();
				var initialStaticTextCount = dataActions.GetAll().Count();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				WaitForIndexing();

				// check created item
				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(((User)item.Creator).Id, ((User)storedItem.Creator).Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);

				Assert.AreEqual(initialStaticTextCount + 1, dataActions.GetAll().Count());

				using (var tran = RepositoryFactory.StartTransaction())
				{
					crudActions.Delete(item);
					tran.Commit();
				}

				WaitForIndexing();

				Assert.AreEqual(initialStaticTextCount, dataActions.GetAll().Count());
			}
		}

		[TestMethod]
		public void Data_get_by_id()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();

				// check existing
				foreach (StaticText staticText in _testData)
				{
					var statData = dataActions.Get(staticText.Id);

					Assert.IsNotNull(statData);
					Assert.AreEqual((object) staticText.Id, statData.Id);
					Assert.AreEqual(staticText.Comments == null ? 0 : staticText.Comments.Count, statData.CommentCounts);
				}

				// check not existing
				var statDataNotExisting = dataActions.Get(Guid.NewGuid());

				Assert.IsNull(statDataNotExisting);
			}
		}

		[TestMethod]
		public void Data_get_by_friendly_url()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();

				// check existing
				foreach (StaticText staticText in _testData)
				{
					var statData = dataActions.Get(staticText.FriendlyUrl);

					Assert.IsNotNull(statData);
					Assert.AreEqual((object) staticText.Id, statData.Id);
					Assert.AreEqual((object) staticText.FriendlyUrl, statData.FriendlyUrl);
					Assert.AreEqual(staticText.Comments == null ? 0 : staticText.Comments.Count, statData.CommentCounts);
				}

				// check not existing
				var statDataNotExisting = dataActions.Get("not-a-text");

				Assert.IsNull(statDataNotExisting);
			}
		}

		[TestMethod]
		public void Data_get_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();

				var dataItems = dataActions.GetAll().OrderBy(item => item.FriendlyUrl).ToArray();
				var originalItems = _testData.OrderBy(item => item.FriendlyUrl).ToArray();

				Assert.AreEqual(dataItems.Length, originalItems.Length);

				for (int i = 0; i < _testData.Length; i++)
				{
					var dataItem = dataItems[i];
					var originalItem = originalItems[i];

					Assert.AreEqual((object) originalItem.Id, dataItem.Id);
					Assert.AreEqual((object) originalItem.FriendlyUrl, dataItem.FriendlyUrl);
					Assert.AreEqual(originalItem.Comments == null ? 0 : originalItem.Comments.Count, dataItem.CommentCounts);
				}
		   }
		}

		[TestMethod]
		public void Data_get_by_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();

				var testData = dataActions.GetAll().OrderBy(item => item.FriendlyUrl).ToArray();
				var data = dataActions.GetByIds(testData.Select(item => item.Id)).OrderBy(item => item.FriendlyUrl).ToArray();

				Assert.AreEqual(testData.Length, data.Length);

				for (int i = 0; i < data.Length; i++)
				{
					Assert.AreEqual(testData[i].Id, data[i].Id);
					Assert.AreEqual(testData[i].Description, data[i].Description);
				}
			}
		}

		[TestMethod]
		public void Data_test_pivot()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextData>();

				var dataItems = dataActions.GetAll(new DateTime(2002, 2, 1)).OrderBy(item => item.PublishDate).ToArray();

				Assert.AreEqual(2, dataItems.Length);
				Assert.AreEqual((object) _testData[0].Id, dataItems[0].Id);
				Assert.AreEqual((object) _testData[1].Id, dataItems[1].Id);

				var itemById = dataActions.Get(_testData[2].Id, new DateTime(2002, 2, 1));
				Assert.IsNull(itemById);

				var itemFriendlyUrl = dataActions.Get(_testData[2].FriendlyUrl, new DateTime(2002, 2, 1));
				Assert.IsNull(itemFriendlyUrl);

				using (var tran = RepositoryFactory.StartTransaction())
				{
					var crudAction = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();
					var itemToDelete = crudAction.FindByPk(_testData[1].Id);
					crudAction.Delete(itemToDelete);
					tran.Commit();
				}

				WaitForIndexing();

				itemById = dataActions.Get(_testData[1].Id, DateTime.Now);
				Assert.IsNull(itemById);

				itemFriendlyUrl = dataActions.Get(_testData[1].FriendlyUrl, DateTime.Now);
				Assert.IsNull(itemFriendlyUrl);
			}
		}
	}
}
