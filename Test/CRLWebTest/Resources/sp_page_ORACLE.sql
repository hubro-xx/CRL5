create or replace package pkg_query as
type cur_query is ref cursor;
end;
--��ҳ�洢����

create or replace procedure sp_page(
  query_ in varchar2,
  fields_ in varchar2,     
  sort_ in  varchar2,   
  pageSize_ in number,
  pageIndex_ in number,  
  count_ out number,
v_Cursor out pkg_query.cur_query) is

strSql varchar2(2000);--��ȡ���ݵ�sql���
pageCount number;--�������¼�¼ҳ��
startIndex number;--��ʼ��¼
endIndex number;--������¼


begin
  strSql:='select count(1) from '||query_;
  EXECUTE IMMEDIATE strSql INTO count_;
  --�������ݼ�¼��ʼ�ͽ���
  pageCount:=count_/pageSize_+1;
  startIndex:=(pageIndex_-1)*pageSize_+1;
  endIndex:=pageIndex_*pageSize_;
  
  strSql:='select rownum ro,'||fields_||' from '||query_||'';  
  strSql:=strSql||' and rownum<='||endIndex;
  
  
  if  sort_ is not null or sort_<>'' then 
     strSql:=strSql||' order by '||sort_;
  end if;
  
  strSql:='select * from ('||strSql||') where ro >='||startIndex;  
  DBMS_OUTPUT.put_line(strSql);

  OPEN v_Cursor FOR strSql; 
end;


