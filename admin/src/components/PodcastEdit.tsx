import { SimpleForm, Edit, TextInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const PodcastEdit = (props: PodcastCreateProps) => {
   return (
      <Edit title="Edit Podcast" {...props}>
         <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="title" />
            <TextInput source="description" />
            <TextInput source="categoryId" />
            <TextInput source="tags" />
            <TextInput source="posterImg" />
            <TextInput source="audioFileUrl" />
            <TextInput source="videoFileUrl" />
            <TextInput source="publisherId" />
         </SimpleForm>
      </Edit>
   );
};

export default PodcastEdit;
