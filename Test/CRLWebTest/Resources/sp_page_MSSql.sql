
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_page]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].sp_page
GO
--���ҳ
CREATE PROCEDURE [dbo].sp_page
( 
	@query_    nvarchar(max)='',    ----��ѯ table1 where a=1  
    @fields_ nvarchar(1000)='*',        ----Ҫ��ʾ���ֶ��б�
    @sort_    nvarchar(100)='',    ----�����������,һ��Ҫ��asc����desc,��@singleSortType���򷽷���Ч
    @pageSize_ int = 10,                ----ÿҳ��ʾ�ļ�¼����
    @pageIndex_    int = 1,                ----Ҫ��ʾ��һҳ�ļ�¼
    @count_    int =1 output           ----��ѯ���ļ�¼��
) 
--�������� @pageSize,@pageIndex
AS
set  nocount  on
declare @sqlTmp nvarchar(max)
declare @sqlGetCount nvarchar(max)
declare @start nvarchar(20) 
declare @end nvarchar(20)
declare @pageCount INT

begin

    --��ȡ��¼��
	  set @sqlGetCount = 'select @Counts=count(*) from ' + @query_


    ----ȡ�ò�ѯ���������-----
    exec sp_executesql @sqlGetCount,N'@Counts int out ',@count_ out
    
    if @count_ = 0
        set @count_ = 1

    --ȡ�÷�ҳ����
    set @pageCount=(@count_+@pageSize_-1)/@pageSize_

    /**��ǰҳ������ҳ�� ȡ���һҳ**/
    if @pageIndex_>@pageCount
        set @pageIndex_=@pageCount

	--���㿪ʼ�������к�
	set @start = @pageSize_*(@pageIndex_-1)+1
	set @end = @start+@pageSize_-1 
	
	set @sqlTmp='SELECT * FROM (select '+@fields_+',ROW_NUMBER() OVER ( Order by '+@sort_+' ) AS RowNumber From '+@query_+') T WHERE T.RowNumber BETWEEN '+@start+' AND '+@end+' order by '+ @sort_
	exec sp_executesql @sqlTmp
end
