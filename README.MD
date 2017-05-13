# Backend
这个是数据库项目的后端，使用RESTful的设计原则，来处理外部的Http请求、访问数据库并完成相应的业务。使用Oracle作为数据库，技术框架为asp.net以及Entity Framework，使用Rider进行开发。

## 项目目录
- ```.git/ .gitignore``` git的仓库以及忽略的配置文件  
- ```Backend.sln Backend.csproj``` 解决方案及项目文件  
- ```Global.asax(.cs)``` asp.net的全局配置文件  
- ```packages(.config)``` nuget程序包及配置文件  


- ```Biz/``` 业务层，用于处理业务
- ```Controller/``` web层，用于处理Http请求
- ```Model/``` 数据层，用于定义、访问数据库
- ```Sample/``` 示例代码，包含上述的三层模型，实现了简单的插入数据的功能  
- ```DbAPI.paw``` 用于Paw的WebApi测试，包含Local和Remote，分别本地及远程的测试

## 运行示例
用rider打开这个项目，点击运行，打开dbAPI.paw，运行Local的Sample，查看运行结果。