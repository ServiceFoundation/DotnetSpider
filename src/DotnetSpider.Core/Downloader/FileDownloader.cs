﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace DotnetSpider.Core.Downloader
{
	/// <summary>
	/// Download from local filesystem.
	/// </summary>
	/// <summary xml:lang="zh-CN">
	/// 从本地文件中下载内容
	/// </summary>
	public class FileDownloader : BaseDownloader
	{
		/// <summary>
		/// Download from local filesystem.
		/// </summary>
		/// <summary xml:lang="zh-CN">
		/// 从本地文件中下载内容
		/// </summary>
		/// <param name="request">请求信息 <see cref="Request"/></param>
		/// <param name="spider">爬虫 <see cref="ISpider"/></param>
		/// <returns>页面数据 <see cref="Page"/></returns>
		protected override Task<Page> DowloadContent(Request request, ISpider spider)
		{
			Console.WriteLine(request.Uri.LocalPath);
			var filePath = request.Uri.LocalPath;
			if (!Env.IsWindows)
			{
				filePath = filePath.Replace("\\", "/");
			}
			if (filePath.StartsWith("\\"))
			{
				filePath = filePath.Substring(2, filePath.Length - 2);
			}
			if (!string.IsNullOrWhiteSpace(filePath))
			{
				if (Env.IsWindows && !filePath.Contains(":"))
				{
					filePath = Path.Combine(Env.BaseDirectory, filePath);
				}
				if (File.Exists(filePath))
				{
					return Task.FromResult(new Page(request)
					{
						Content = File.ReadAllText(filePath)
					});
				}
			}
			var msg = $"File {filePath} unfound.";
			Page page = new Page(request)
			{
				Exception = new FileNotFoundException(msg),
				Skip = true
			};

			spider.Logger.Error($"Download {request.Url} failed: {msg}.");
			return Task.FromResult(page);
		}
	}
}