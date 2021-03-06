﻿using System.Collections.Generic;
using System.Linq;
using DotnetSpider.Core;
using DotnetSpider.Extension.Model;

namespace DotnetSpider.Extension.Pipeline
{
	/// <summary>
	/// 内存数据管道, 把所有数据结果存到内存列表中
	/// </summary>
	public class CollectionEntityPipeline : ModelPipeline, ICollectionEntityPipeline
	{
		private readonly Dictionary<string, List<dynamic>> _collector = new Dictionary<string, List<dynamic>>();
		private readonly object _locker = new object();

		/// <summary>
		/// 取得实体名称的所有数据
		/// </summary>
		/// <param name="modeIdentity">爬虫实体名称</param>
		/// <returns>实体数据</returns>
		public IEnumerable<dynamic> GetCollection(string modeIdentity)
		{
			lock (_locker)
			{
				if (_collector.TryGetValue(modeIdentity, out var result))
				{
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// 存储页面解析器解析到的数据结果到内存中
		/// </summary>
		/// <param name="entityName">爬虫实体类的名称</param>
		/// <param name="datas">实体类数据</param>
		/// <param name="spider">爬虫</param>
		/// <returns>最终影响结果数量(如数据库影响行数)</returns>
		protected override int Process(IModel model, IEnumerable<dynamic> datas, ISpider spider)
		{
			lock (_locker)
			{
				if (_collector.ContainsKey(model.Identity))
				{
					var list = _collector[model.Identity];
					list.AddRange(datas);
				}
				else
				{
					var list = new List<dynamic>();
					list.AddRange(datas);
					_collector.Add(model.Identity, list);
				}
				return datas.Count();
			}
		}
	}
}
