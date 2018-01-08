using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FireForms.Database.Exceptions;

namespace FireForms.Database
{
    public class FirebaseDoc<T>
    {

        public FirebaseDoc(FirebaseDatabase firebaseDatabase, string localDBpath)
        {
            Init(localDBpath);
            this.firebaseDatabase = firebaseDatabase;

        }

        private FirebaseDatabase firebaseDatabase;

        public void SetFirebaseDatabase(FirebaseDatabase value)
        { firebaseDatabase = value; }

        public Dictionary<string, FirebaseDoc<object>> DbRef { get; set; }

        public LiteCollection<T> Collection { get; set; }

        private BsonMapper mapper;

        public string AccessToken { get; set; }

        private string localDBpath;

        private LiteDatabase liteDatabase;

        private void Init(string localDBpath)
        {
            this.localDBpath = localDBpath;
            using (liteDatabase = new LiteDatabase(localDBpath))
            {
                Collection = liteDatabase.GetCollection<T>(typeof(T).Name);
            }
        }

        private ObservableCollection<T> observableCollection;

        public ObservableCollection<T> AsObservableCollection()
        {
            observableCollection = new ObservableCollection<T>(Collection.FindAll());
            return observableCollection;
        }

        private bool isListening;

        public async Task Listen()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
            };
            var httpClient = new HttpClient(handler, true)
            {
                Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite),
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            isListening = true;
            while (isListening)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, firebaseDatabase.FullUri);
                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    string operation = "";
                    var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using (var reader = new StreamReader(stream))
                    {
                        while (isListening)
                        {

                            var line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                await Task.Delay(2000).ConfigureAwait(false);
                                continue;
                            }
                            var tuple = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                            switch (tuple[0].ToLower())
                            {
                                case "event":
                                    operation = tuple[1];
                                    break;
                                case "data":
                                    this.ProcessServerData(operation, tuple[1]);
                                    break;
                            }

                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("************************************************************");
                    Debug.WriteLine(ex.ToString());
                    Debug.WriteLine("************************************************************");
                    await Task.Delay(2000).ConfigureAwait(false);
                }

            }

        }

        private void ProcessServerData(string serverEvent, string serverData)
        {
            switch (serverEvent)
            {
                case "put":
                    {
                        var result = JObject.Parse(serverData);
                        var path = result["path"].ToString();
                        var data = result["data"].ToString();
                        if (string.IsNullOrWhiteSpace(data))
                        {
                            Collection.Delete(path);
                        }
                        else
                        {
                            
                            Collection.Upsert(JsonConvert.DeserializeObject<T>(data));
                        }
                        break;
                    }
                    
                case "patch":
                    {
                        var result = JObject.Parse(serverData);
                        var path = result["path"].ToString();
                        var data = result["data"].ToString();
                        if (string.IsNullOrWhiteSpace(data))
                        {
                            Collection.Delete(path);
                        }
                        else
                        {
                            Collection.Upsert(JsonConvert.DeserializeObject<T>(data));
                        }
                        break;
                    }
                    
                case "keep-alive":
                    break;
                case "cancel":
                    isListening = false;
                    throw new OperationCanceledException();
                case "auth_revoked":
                    isListening = false;
                    throw new OperationCanceledException();
            }
        }

        public void StopListening()
        {
            isListening = false;
        }
        

        public void DeleteAll()
        {
            liteDatabase.DropCollection(typeof(T).Name);

        }

        public async Task<StatusEnum> UpsertAsync(T obj)
        {
            mapper = BsonMapper.Global;
            Collection.Upsert(obj);
            var mobj = BsonMapper.Global.ToDocument<T>(obj);
            HttpClient client = new HttpClient();
            var response = await client.PutAsync($"{firebaseDatabase.DatabaseURL}/{firebaseDatabase.Target}/{mobj["_id"].AsString}.json", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return StatusEnum.ALLSUCCESS;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return StatusEnum.LOCALSAVED;
                }
                return StatusEnum.LOCALSAVED;

            }

        }

        public async Task DeleteAsync(T obj)
        {
            var mobj = BsonMapper.Global.ToDocument<T>(obj);
            HttpClient client = new HttpClient();
            var response = await client.DeleteAsync($"{firebaseDatabase.DatabaseURL}/{firebaseDatabase.Target}/{mobj["_id"].AsString}.json").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Collection.Delete(mobj["_id"].AsString);
            }
            else
            {
                throw FireFormsException.from(response.StatusCode);
            }
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var mobj = BsonMapper.Global.ToDocument<T>(Collection.FindOne(predicate));
            HttpClient client = new HttpClient();
            var response = await client.DeleteAsync($"{firebaseDatabase.DatabaseURL}/{firebaseDatabase.Target}/{mobj["_id"].AsString}.json").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Collection.Delete(mobj["_id"].AsString);
            }
            else
            {
                throw FireFormsException.from(response.StatusCode);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            Dictionary<string, T> obj;
            IEnumerable<T> values;
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetAsync(firebaseDatabase.FullUri).ConfigureAwait(false);
                var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (responseData == "null")
                {
                    return null;
                }
                if (!response.IsSuccessStatusCode){
                    throw FireFormsException.from(response.StatusCode);
                }
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                try
                {
                    obj = JsonConvert.DeserializeObject<Dictionary<string, T>>(responseData, settings);
                    values = obj.Values;
                }
                catch (JsonSerializationException)
                {
                    var arr = JsonConvert.DeserializeObject<IEnumerable<T>>(responseData, settings);
                    values = arr.Where(x => x != null).ToArray<T>();
                }
                try
                {
                    Collection.Upsert(values);
                }
                catch (Exception)
                {
                    throw;
                }
                return values;

            }
            catch (Exception)
            {
                values = Collection.FindAll();

                if (values == null)
                {
                    throw FireFormsException.from(System.Net.HttpStatusCode.NotFound);
                }
                return values;
            }
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            T obj;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(firebaseDatabase.DatabaseURL).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                obj = JsonConvert.DeserializeObject<T>(responseData, settings);
                Collection.Upsert(obj);
                return obj;
            }
            else
            {
                obj = Collection.FindOne(predicate);
                if (obj == null)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw FireFormsException.from(response.StatusCode);
                    }
					
                }
                return obj;
            }
        }




        public int Sync()
        {
            throw new NotImplementedException();
        }
    }
}
