export const getUsers = async (accessToken) => {
    const res = await fetch("http://localhost:5098/api/admin/users", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Failed to fetch users");
    }
  
    return res.json();
  };
  
  export const getUnpaidBills = async (accessToken) => {
    const res = await fetch("http://localhost:5098/api/admin/unpaid-bills", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Failed to fetch unpaid bills");
    }
  
    return res.json();
  };
  
  export const blockUser = async (Id, accessToken) => {
    const res = await fetch(`http://localhost:5098/api/admin/block/${Id}`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Failed to block user");
    }
  };
  
  export const unblockUser = async (Id, accessToken) => {
    const res = await fetch(`http://localhost:5098/api/admin/unblock/${Id}`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Failed to unblock user");
    }
  };

  export const addUser = async (auth0Id, name, phoneNumber, email, accessToken) => {
    const res = await fetch(`http://localhost:5098/api/admin/add-user`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ auth0Id, name, phoneNumber, email }),
    });
  
    if (!res.ok) {
      throw new Error("Error when adding the subscriber");
    }
  
    return res.json();
  };
  