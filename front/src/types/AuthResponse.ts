interface AuthResponse {
   email: string;
   fullName: string;
   id: number;
   role: string;
   username: string;
   code?: number;
}

export default AuthResponse;
