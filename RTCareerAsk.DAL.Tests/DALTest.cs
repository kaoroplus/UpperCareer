﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVOSCloud;
using RTCareerAsk.DAL.Domain;

namespace RTCareerAsk.DAL.Tests
{
    [TestClass]
    public class DALTest
    {
        #region Property

        public LeanCloudAccess LCDal { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PostId { get; set; }
        public string AnswerId { get; set; }

        #endregion

        #region Initialization

        public void Initialization()
        {
            AVClient.Initialize("5ptNj5fF9TplwYYNYo34Ujmi-gzGzoHsz", "oxEMyVyz3XmlI8URg87Xp1l5");
        }

        [TestInitialize]
        public void TestInitiation()
        {
            LCDal = new LeanCloudAccess();
            Initialization();

            UserId = "578101cad342d30057c928a5";
            UserName = "alex.lai.wei";
            Password = "r00716630";

            PostId = "578171ea5bbb500061faf102";
            AnswerId = "5788f0d8128fe1006398b565";
        }

        #endregion

        #region Upper Test

        [TestMethod]
        public async Task RegisterUserTest()
        {
            User u1 = new User()
            {
                Password = "r00716630",
                Email = "42379442@qq.com",
                Name = "老司机阿来",
            };

            User u2 = new User()
            {
                Password = "r00716630",
                Email = "alex.lai.wei@gmail.com",
                Name = "呆萌神棍",
            };

            Task<bool> t1 = LCDal.RegisterUser(u1);
            Task<bool> t2 = LCDal.RegisterUser(u2);

            await Task.WhenAll(t1, t2);

            Assert.IsFalse(t1.Result);
            Assert.IsTrue(t2.Result);
        }

        [TestMethod]
        public void ValidateUserTest()
        {
            bool isValid = LCDal.ValidateUser(UserName, Password).Result;

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public async Task LoginWithEmail()
        {
            string email = "42379442@qq.com";
            string password = "r00716630";

            User u = await LCDal.LoginWithEmail(email, password);

            Assert.AreEqual("老司机阿来", u.Name);
        }

        [TestMethod]
        public async Task AssignRolesToUserTest()
        {
            string targetUserId = "5796c9640a2b580061cf61e2";

            IEnumerable<string> roleIds = new List<string>() { "Admin" };

            Assert.IsTrue(await LCDal.AssignRolesToUser(targetUserId, roleIds));
        }

        [TestMethod]
        public async Task RemoveRolesFromUserTest()
        {
            IEnumerable<string> roleIds = new List<string>() { "57810a335bbb500061f8b202", "57810a392e958a0054c3b763" };

            Assert.IsTrue(await LCDal.RemoveRolesFromUser(UserId, roleIds));
        }

        [TestMethod]
        public async Task BlockUserTest()
        {
            Assert.IsTrue(await LCDal.BlockUser(UserId));
        }

        [TestMethod]
        public async Task UnblockUserTest()
        {
            Assert.IsTrue(await LCDal.UnblockUser(UserId));
        }

        [TestMethod]
        public async Task IfAlreadyFollowedTest()
        {
            string targetId = "578a84278ac24700608b9f21";

            Assert.IsTrue(await LCDal.IfAlreadyFollowed(UserId, targetId));
        }

        [TestMethod]
        public async Task NewFollowTest()
        {
            string followeeId = "578a84278ac24700608b9f21";

            Assert.IsTrue(await LCDal.Follow(UserId, followeeId));
        }

        [TestMethod]
        public async Task GetFollowerCountTest()
        {
            Assert.AreEqual(0, await LCDal.GetFollowerCount(UserId));
        }

        [TestMethod]
        public async Task GetFolloweeCountTest()
        {
            Assert.AreEqual(1, await LCDal.GetFolloweeCount(UserId));
        }

        [TestMethod]
        public async Task GetFollowersTest()
        {
            Assert.AreEqual(0, await LCDal.GetFollowers(UserId).ContinueWith(t => t.Result.Count()));
        }

        [TestMethod]
        public async Task GetFolloweesTest()
        {
            IEnumerable<User> followees = await LCDal.GetFollowees(UserId);

            Assert.AreEqual(1, followees.Count());
            Assert.AreEqual("东北大脑袋", followees.First().Name);
        }

        [TestMethod]
        public async Task UnfollowTest()
        {
            string followeeId = "578a84278ac24700608b9f21";

            Assert.IsTrue(await LCDal.Unfollow(UserId, followeeId));
        }

        [TestMethod]
        public async Task LoadMessagesForUserTest()
        {
            string userId = "578a84278ac24700608b9f21";
            List<Message> messages = await LCDal.LoadMessagesForUser(userId).ContinueWith(t => t.Result.ToList());

            Assert.AreEqual(4, messages.Count());
            Assert.AreEqual(2, messages.Where(x => x.IsNew).Count());
            //Assert.AreEqual("578f4ca5165abd00674860fa", messages[0].Content.ObjectID);
            //Assert.AreEqual("578f4c3b8ac2470060b93f60", messages[1].Content.ObjectID);
            //Assert.AreEqual("578f4bb08ac2470060b92d7b", messages[2].Content.ObjectID);
        }

        [TestMethod]
        public async Task CountNewMessageForUserTest()
        {
            string userId = "578a84278ac24700608b9f21";

            Assert.AreEqual(2, await LCDal.CountNewMessageForUser(userId));
        }

        [TestMethod]
        public async Task WriteNewMessageTest()
        {
            User from = new User();
            User to = new User() { ObjectID = "578a84278ac24700608b9f21" };
            //User to = new User() { ObjectID = "578a84278ac24700608b9f21" };
            MessageBody msgBody = new MessageBody()
            {
                Title = "您的评论被回复",
                Content = "您好，你有一条评论被回复，请前往浏览！",
                IsSystem = string.IsNullOrEmpty(to.ObjectID)
            };
            Message msg = new Message()
            {
                Content = msgBody,
                IsNew = true,
                From = from,
                To = to
            };

            Assert.IsTrue(await LCDal.WriteNewMessage(msg));
        }

        [TestMethod]
        public async Task MarkMessageAsOpenedTest()
        {
            string userId = "578a84278ac24700608b9f21";
            string messageId = "578f4ea10a2b58006869235f";

            Assert.IsTrue(await LCDal.MarkMessageAsOpened(userId, messageId));
        }

        [TestMethod]
        public async Task DeleteMessageTest()
        {
            string messageId = "5790bd02a34131005a6371be";
            string userId = "578a84278ac24700608b9f21";

            Assert.IsTrue(await LCDal.DeleteMessage(userId, messageId));
        }

        [TestMethod]
        public async Task FindAllFilesTest()
        {
            IEnumerable<Domain.FileInfo> files = await LCDal.FindAllFiles();

            Assert.AreEqual(3, files.Count());
        }

        [TestMethod]
        public async Task FindFilesByPropertyTest()
        {
            string propKey = "mime_type";
            string propValue = "image/jpeg";

            Assert.AreEqual(1, await LCDal.FindFilesByProperty(propKey, propValue).ContinueWith(t => t.Result.Count()));
        }

        [TestMethod]
        public void EncodeAndDecodeTest()
        {
            string text = "为什么毛豆胖如狗？？为什么？？";

            byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
            string result = System.Text.Encoding.UTF8.GetString(data);

            Assert.AreEqual(text, result);
        }

        [TestMethod]
        public async Task DownloadFileByIDTest()
        {
            string fileId = "578bf6026be3ff006ce0b27e";

            Domain.File fileData = await LCDal.DownloadFileByID(fileId);

            string result = System.Text.Encoding.UTF8.GetString(fileData.FileDataByte);

            Assert.AreEqual("为什么毛豆胖如狗？？为什么？？", result);
        }

        [TestMethod]
        public void FindPostObjectsTest()
        {
            Assert.AreEqual(1, LCDal.FindQuestionList().Result.Count());
        }

        [TestMethod]
        public async Task SaveAnswerTest()
        {
            string ansContent = "……原来问题是我自己提的，这他妈就有点尴尬了。";
            Answer ans = new Answer()
            {
                Content = ansContent,
                CreatedBy = new User() { ObjectID = UserId },
                ForQuestion = new Question { ObjectID = PostId }
            };

            Assert.IsTrue(await LCDal.SaveNewAnswer(ans));
        }

        [TestMethod]
        public async Task SaveNewCommentTest()
        {
            string cmtContent = "还是我自己的帖子，这也是日了犬了……";
            Comment cmt = new Comment()
            {
                Content = cmtContent,
                CreatedBy = new User() { ObjectID = UserId },
                ForAnswer = new Answer() { ObjectID = AnswerId }
            };

            Assert.IsTrue(await LCDal.SaveNewComment(cmt));
        }

        [TestMethod]
        public async Task LoadQuestionListTest()
        {
            IEnumerable<QuestionInfo> qis;

            qis = await LCDal.LoadQuestionList(3, false);
            Assert.AreEqual(0, qis.Count());

            qis = await LCDal.LoadQuestionList(1, false);
            Assert.AreEqual(20, qis.Count());
            Assert.AreEqual(8, qis.First().DateCreate.Day);

            qis = await LCDal.LoadQuestionList(0, true);
            Assert.AreEqual(20, qis.Count());
            Assert.AreEqual(13, qis.First().AnswerCount);
        }

        [TestMethod]
        public async Task LoadAnswerListTest()
        {
            IEnumerable<AnswerInfo> ais;

            ais = await LCDal.LoadAnswerList(3, false);
            Assert.AreEqual(0, ais.Count());

            ais = await LCDal.LoadAnswerList(1, false);
            Assert.AreEqual(18, ais.Count());
            Assert.AreEqual(27, ais.First().DateCreate.Day);

            ais = await LCDal.LoadAnswerList(0, true);
            Assert.AreEqual(20, ais.Count());
            Assert.AreEqual(9, ais.First().CommentCount);
        }

        [TestMethod]
        public async Task LoadBugReportsTest()
        {
            List<Bug> bugs = await LCDal.LoadBugReports();

            Assert.AreEqual(14, bugs.Count);
        }

        [TestMethod]
        public async Task UpdateBugReportTest()
        {
            Bug bug = new Bug()
            {
                ObjectID = "5798715da34131005aaa4fbb",
                StatusCode = 1,
                Priority = 0
            };

            await LCDal.UpdateBugReport(bug);
        }

        [TestMethod]
        public async Task IsInviteCodeValidTest()
        {
            string code1 = "UPPER2016";
            string code2 = "UPPER2015";

            Assert.IsTrue(await LCDal.IsInviteCodeValid(code1));
            Assert.IsFalse(await LCDal.IsInviteCodeValid(code2));
        }

        [TestMethod]
        public async Task LoadUserDetailTest()
        {
            Assert.AreEqual("老司机阿来", await LCDal.LoadUserDetail(UserId).ContinueWith(t => t.Result.ForUser.Name));
        }

        [TestMethod]
        public async Task SaveUserDetailTest()
        {
            UserDetail ud = await LCDal.LoadUserDetail(UserId);

            ud.ForUser.Name = "老司机阿来";
            ud.ForUser.Title = "全国失眠达人";
            ud.ForUser.Gender = 1;
            ud.ForUser.Company = "拉夫德尔网络科技";
            ud.SelfDescription = "激动得不知道说些什么好";

            Assert.IsTrue(await LCDal.SaveUserDetail(ud));
        }

        [TestMethod]
        public async Task DeleteAnswerWithCommentsTest()
        {
            string ansId = "57bd740cdf0eea005c6fa262";

            Assert.IsTrue(await LCDal.DeleteAnswerWithComments(ansId));
        }

        [TestMethod]
        public async Task PerformVoteTest()
        {
            Vote v = new Vote()
            {
                TargetID = "57d4265e7db2a20068352cfa",
                Type = VoteType.Answer,
                VoterID = UserId,
                IsLike = true,
                IsUpdate = false
            };

            Assert.IsTrue(await LCDal.PerformVote(v));
        }

        #endregion

        #region Practice Test

        [TestMethod]
        public async Task CreateTestCommentTst()
        {
            string content = "测试内容3";

            Assert.IsTrue(await LCDal.CreateTestComment(content, UserId));
        }

        [TestMethod]
        public async Task CreateTestAnswerTst()
        {
            string content = "测试回答2";

            Assert.IsTrue(await LCDal.CreateTestAnswer(content, UserId));
        }

        [TestMethod]
        public async Task CreateNewTableTst()
        {
            string tableName = "Message";
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("from", await LCDal.GetUserByID(UserId));
            properties.Add("to", null);
            properties.Add("content", await LCDal.GetMessageBodyByID("578f4bb08ac2470060b92d7b"));
            properties.Add("isNew", true);

            Assert.IsTrue(await LCDal.CreateNewTable(tableName, properties));
        }

        [TestMethod]
        public async Task GenerateNewInviteCodeTst()
        {
            string code = "2016";

            IEnumerable<string> codes = await LCDal.GenerateNewInviteCode(true, new List<string>() { code });

            Assert.AreEqual(0, codes.Where(x => x != null).Count());
        }

        [TestMethod]
        public async Task DownloadFileTst()
        {
            byte[] result = await LCDal.DownloadFile();

            File f = new File();
        }

        [TestMethod]
        public async Task RelationSettingTst()
        {
            await LCDal.RelationSetting();
        }

        [TestMethod]
        public async Task SearchByRelationTst()
        {
            IEnumerable<AVObject> objs = await LCDal.SearchByRelation();

            Assert.AreEqual("Test", objs.First().ClassName);
            Assert.AreEqual(1, objs.Count());
            Assert.AreEqual("test", objs.First().Get<string>("testColumn"));
        }

        [TestMethod]
        public async Task UpdateRelationTst()
        {
            await LCDal.UpdateRelation();
        }

        [TestMethod]
        public async Task DeleteRelation()
        {
            await LCDal.DeleteRelation();
        }

        [TestMethod]
        public async Task UpdateFileTst()
        {
            string fileId = "579871530a2b580061e45b51";

            await LCDal.UpdateFile(fileId);
        }

        [TestMethod]
        public async Task FindFileByUrlTst()
        {
            string url = await LCDal.LoadUserInfo(UserId).ContinueWith(t => t.Result.Portrait);
            string uri = "http://ac-5ptNj5fF.clouddn.com/e80ab7e9-cf14-47fa-88eb-08924b2dbe13";
            AVObject file = await LCDal.FindFileByUrl(uri);

            Assert.AreEqual("_File", file.ClassName);
        }

        [TestMethod]
        public async Task UpdateAnswerContentTst()
        {
            string ansId = "57986f3b1532bc0060ee1f5b";
            string content = "我知道错了，下次不写这么长的答案了。";

            Assert.IsTrue(await LCDal.UpdateAnswerContent(ansId, content));
        }

        [TestMethod]
        public async Task GetAnswer100TimesAsyncTst()
        {
            string ansId = "57e222fb0e3dd90069863233";

            await LCDal.GetAnswer100TimesAsync(ansId);
        }

        [TestMethod]
        public async Task GetAnswer100TimesTst()
        {
            string ansId = "57e222fb0e3dd90069863233";

            await LCDal.GetAnswer100Times(ansId);
        }

        [TestMethod]
        public async Task UpdateSubpostCountForQuestionsTst()
        {
            Assert.IsTrue(await LCDal.UpdateSubpostCountForQuestions());
        }

        [TestMethod]
        public async Task UpdateSubpostCountForAnswersTst()
        {
            Assert.IsTrue(await LCDal.UpdateSubpostCountForAnswers());
        }

        #endregion
    }
}
