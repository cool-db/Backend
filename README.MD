# Backend
这个是数据库项目的后端，使用RESTful的设计原则，来处理外部的Http请求、访问数据库并完成相应的业务。使用Oracle作为数据库，技术框架为asp.net以及Entity Framework，使用Rider进行开发。

## 项目目录
- ```.git/ .gitignore``` git的仓库以及忽略的配置文件  
- ```Backend.sln Backend.csproj``` 解决方案及项目文件  
- ```Global.asax(.cs)``` asp.net的全局配置文件  
- ```packages(.config)``` Nuget程序包及配置文件  


- ```Biz/``` 业务层，用于处理业务
- ```Controller/``` Web层，用于处理Http请求
- ```Model/``` 数据层，用于定义、访问数据库
- ```Sample/``` 示例代码，包含上述的三层模型，实现了简单的插入数据的功能  
- ```DbAPI.paw``` 用于Paw的webapi测试，包含Local和Remote，分别本地及远程的测试

## 运行示例
用Rider打开这个项目，点击运行，打开dbAPI.paw，运行Local的Sample，查看运行结果。

## 开发流程

1. 完成Model的设计
2. 完成Controller以及Biz的代码，在实际开发过程中，可能需要回到1重新验证、修改数据库的设计。

## 如何Debug

用Rider打开项目

- Model：完成数据库层代码后，使用Paw测试Local.DebugModel，返回成功后用Rider查看数据库是否符合预期。
- Controller：只有处理Http请求、分发业务的功能，不会产生棘手的bug，使用Paw查看能否处理Http请求即可。
- Biz：完成业务代码后，使用Paw发起Http请求、Controller分发对应的业务，查看结果是否符合预期。

## Http状态码和返回信息

成功时返回```{code:200}```

错误时返回```{code: int,message: string}```，其中，code约定如下：  
- 401 未授权
- 404 未找到

message为出错时的错误信息
