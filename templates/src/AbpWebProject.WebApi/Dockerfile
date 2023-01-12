FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY . /app
EXPOSE 80
#设置时区
#RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo Asia/Shanghai > /etc/timezone	
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /usr/lib/ssl/openssl.cnf
#注意：请将ENTRYPOINT替换为本应用启动dll
ENTRYPOINT ["dotnet", "AbpWebProject.WebApi.dll"]