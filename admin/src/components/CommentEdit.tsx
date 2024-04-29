import { SimpleForm, Edit, TextInput, DateInput } from "react-admin";

interface PodcastCreateProps {
   basePath?: string;
}

export const CommentEdit = (props: PodcastCreateProps) => {
   return (
      <Edit title="Edit Comment" {...props}>
         <SimpleForm>
            <TextInput source="userId" />
            <TextInput source="episodeId" />
            <DateInput source="date" />
            <TextInput source="message" />
         </SimpleForm>
      </Edit>
   );
};

export default CommentEdit;
