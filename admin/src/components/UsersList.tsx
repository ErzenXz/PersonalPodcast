import { List, Datagrid, TextField, EditButton, DeleteButton } from "react-admin";

interface ListProps {
   basePath?: string | undefined;
}

export const UsersList = (props: ListProps) => {
   return (
      <List {...props}>
         <Datagrid>
            <TextField source="id" />
            <TextField source="username" />
            <TextField source="fullName" />
            <TextField source="email" />
            <TextField source="role" />
            <EditButton />
            <DeleteButton />
         </Datagrid>
      </List>
   );
};

export default UsersList;
