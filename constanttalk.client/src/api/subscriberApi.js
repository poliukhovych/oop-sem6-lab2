export const getAvailableServices = async (accessToken) => {
    const res = await fetch("http://localhost:5098/api/subscriber/services/available", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Error fetching available services");
    }
  
    return res.json();
  };
  
  export const getMyServices = async (accessToken) => {
    const res = await fetch("http://localhost:5098/api/subscriber/services/my", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Error fetching your services");
    }
  
    return res.json();
  };
  
  export const addService = async (serviceId, accessToken) => {
    const res = await fetch(`http://localhost:5098/api/subscriber/services/${serviceId}/add`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Error when activating service");
    }
  };
  
  export const getMyBills = async (accessToken) => {
    const res = await fetch("http://localhost:5098/api/subscriber/bills", {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Error fetching bills");
    }
  
    return res.json();
  };
  
  export const payBill = async (billId, accessToken) => {
    const res = await fetch(`http://localhost:5098/api/subscriber/bills/${billId}/pay`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  
    if (!res.ok) {
      throw new Error("Error when paying bill");
    }
  };