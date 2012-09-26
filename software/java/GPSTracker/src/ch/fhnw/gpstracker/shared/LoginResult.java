package ch.fhnw.gpstracker.shared;

import com.google.gwt.user.client.rpc.IsSerializable;

public class LoginResult implements IsSerializable
{
	private Integer userid;
		
	public LoginResult() 
	{
	}
	
	public LoginResult(Integer id) 
	{
		this.userid = id;
	}

	public boolean isSuccessful() {
		return (userid != null);
	}
	
	public Integer getUserID() {
		return userid;
	}
	
}
