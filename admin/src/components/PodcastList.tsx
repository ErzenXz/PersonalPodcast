import {
   List,
   Datagrid,
   TextField,
   DateField,
   ImageField,
   EditButton,
   DeleteButton,
} from "react-admin";

interface PodcastListProps {
   basePath?: string | undefined;
}

export const PodcastList = (props: PodcastListProps) => {
   return (
      <List {...props}>
         <Datagrid>
            <TextField source="id" />
            <TextField source="title" />
            <TextField source="description" />
            <TextField source="categoryId" />
            <ImageField source="posterImg" />
            <DateField source="createdDate" />
            <EditButton />
            <DeleteButton />
         </Datagrid>
      </List>
   );
};

export default PodcastList;
