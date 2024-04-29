import { List, Datagrid, TextField, EditButton, DeleteButton } from "react-admin";

interface ListProps {
   basePath?: string | undefined;
}

export const ListComments = (props: ListProps) => {
   return (
      <List {...props}>
         <Datagrid>
            <TextField source="id" />
            <TextField source="userId" />
            <TextField source="episodeId" />
            <TextField source="date" />
            <TextField source="message" />
            <EditButton />
            <DeleteButton />
         </Datagrid>
      </List>
   );
};

export default ListComments;
