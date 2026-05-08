import { ComponentDetailClient } from "@/components/components/ComponentDetailClient";

export default async function ComponentDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <ComponentDetailClient componentId={Number(id)} />;
}
