using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public FirebaseDatabase FirebaseDatabase
        {
            //get { return firebaseDatabase; }
            set { firebaseDatabase = value; }
        }

        private Dictionary<string, FirebaseDoc<object>> dbRef;

        public Dictionary<string, FirebaseDoc<object>> DbRef
        {
            get { return dbRef; }
            set { dbRef = value; }
        }


        /*public FirebaseDoc(FirebaseDatabase firebaseDatabase, string localDBpath)
        {
            Init(localDBpath);
            this.firebaseDatabase = firebaseDatabase;
            
            
        }*/

        private LiteCollection<T> collection;

        public LiteCollection<T> Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        private BsonMapper mapper;

        private string _accessToken;

        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        private string localDBpath;

        private LiteDatabase liteDatabase;

        private void Init(string localDBpath)
        {
            this.localDBpath = localDBpath;
            using (liteDatabase = new LiteDatabase(localDBpath))
            {
                collection = liteDatabase.GetCollection<T>(typeof(T).Name);
            }
        }

        public void IncludeCObject<K>(Expression<Func<T, K>> dbref, string variableName, T obj)
        {
            //adasdsa
            var mapper = BsonMapper.Global;
            mapper.Entity<T>().DbRef(dbref);
            var dyn = mapper.ToDocument(obj);
            var teste = dyn;

            /*BsonMapper.Global.RegisterType<K>
            (
                serialize: (kobj) => BsonMapper.Global.ToDocument<K>(kobj)["_id"].AsString,
                deserialize: (bson) => GetAsync(dbref)
            );
            /*
            MemberMapper memberMapper = new MemberMapper();
            
            mapper.Entity<T>().DbRef<K>(dbref);
            DbRef.Add(variableName, obj);
            mapper.Entity<T>().i*/
            //collection = collection.Include(dbref);


        }

        public void DeleteAll()
        {
            liteDatabase.DropCollection(typeof(T).Name);
            
        }

        public async Task<StatusEnum> UpsertAsync(T obj)
        {
            mapper = BsonMapper.Global;            
            collection.Upsert(obj);
            var mobj = BsonMapper.Global.ToDocument<T>(obj);
            HttpClient client = new HttpClient();
            var response = await client.PutAsync(firebaseDatabase.DatabaseURL + "/" + mobj["_id"].AsString + ".json", new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")).ConfigureAwait(false);
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
            var response = await client.DeleteAsync(firebaseDatabase.DatabaseURL + "/" + mobj["_id"].AsString + ".json").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Collection.Delete(mobj["_id"].AsString);
            }
            else
            {
                throw new Exception("Not found or no internet access(No cached instance found too)");
            }
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var mobj = BsonMapper.Global.ToDocument<T>(Collection.FindOne(predicate));
            HttpClient client = new HttpClient();
            var response = await client.DeleteAsync(firebaseDatabase.DatabaseURL + "/" + mobj["_id"].AsString + ".json").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Collection.Delete(mobj["_id"].AsString);
            }
            else
            {
                throw new Exception("Not found or no internet access(No cached instance found too)");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            Dictionary<string, T> obj;
            IEnumerable<T> values;
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetAsync(firebaseDatabase.DatabaseURL + ".json").ConfigureAwait(false);
                var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (responseData == "null")
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
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
                catch (JsonSerializationException ex)
                {
                    var arr = JsonConvert.DeserializeObject<IEnumerable<T>>(responseData, settings);
                    values = arr.Where(x => x != null).ToArray<T>();
                }
                try
                {
                    Collection.Upsert(values);
                }
                catch (Exception e)
                {
                    throw e;
                }
                return values;

            }
            catch (Exception ex)
            {
                values = Collection.FindAll();
                if (values == null)
                {
                    throw new Exception("Not found or no internet access(No cached instance found too)");
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
                    throw new Exception("Not found or no internet access(No cached instance found too)");
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
