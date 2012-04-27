using System.Linq;
using Dulcet.Twitter.Rest;
using Inscribe.Authentication;
using Inscribe.Common;
using Inscribe.Storage;
using System.Threading.Tasks;

namespace Inscribe.Communication
{
    public static class NotScheduledReceiving
    {
        public static void ReceiveUpstreamHome(AccountInfo a, long maxId)
        {
            Task.Factory.StartNew(() =>
                ApiHelper.ExecApi(() =>
                    a.GetHomeTimeline(maxId: maxId, count: TwitterDefine.HomeReceiveMaxCount)
                        .ForEach(s => TweetStorage.Register(s))
                )
            );
        }

        public static void ReceiveUpstreamMentions(AccountInfo a, long maxId)
        {
            Task.Factory.StartNew(() =>
                ApiHelper.ExecApi(() =>
                    a.GetMentions(maxId: maxId, count: TwitterDefine.MentionReceiveMaxCount)
                        .ForEach(s => TweetStorage.Register(s))
                )
            );
        }

        public static void ReceiveUpstreamDirectMessages(AccountInfo a, long maxId)
        {
            Task.Factory.StartNew(() =>
                ApiHelper.ExecApi(() =>
                    a.GetDirectMessages(maxId: maxId, count: TwitterDefine.DmReceiveMaxCount)
                        .ForEach(s => TweetStorage.Register(s))
                )
            );

            Task.Factory.StartNew(() =>
                ApiHelper.ExecApi(() =>
                    a.GetSentDirectMessages(maxId: maxId, count: TwitterDefine.DmReceiveMaxCount)
                        .ForEach(s => TweetStorage.Register(s))
                )
            );
        }

        public static void ReceiveUpstreamListStatuses(string list, long maxId)
        {
            Task.Factory.StartNew(() =>
            {
                string[] sp = list.Split('/');
                string listUserName = sp[0];
                string listName = sp[1];

                ApiHelper.ExecApi(() =>
                    (AccountStorage.Get(listUserName) ?? AccountStorage.GetRandom())
                        .GetListStatuses(listUserName, listName, maxId: maxId.ToString(), perPage: TwitterDefine.ListReceiveCount, includeRts: true, includeEntities: true)
                        .ForEach(s => TweetStorage.Register(s))
                );
            });
        }
    }
}
