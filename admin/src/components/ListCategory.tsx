import { List, Datagrid, TextField, EditButton, DeleteButton } from "react-admin";

interface ListProps {
   basePath?: string | undefined;
}

export const ListCategory = (props: ListProps) => {
   return (
      <List {...props}>
         <Datagrid>
            <TextField source="id" />
            <TextField source="name" />
            <EditButton />
            <DeleteButton />
         </Datagrid>
      </List>
   );
};

export default ListCategory;
